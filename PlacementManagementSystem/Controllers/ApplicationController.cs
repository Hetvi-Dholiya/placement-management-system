using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PlacementManagementSystem.Data;
using PlacementManagementSystem.Helpers;
using PlacementManagementSystem.Models;
using PlacementManagementSystem.ViewModels;

namespace PlacementManagementSystem.Controllers
{
    public class ApplicationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ApplicationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // 1. STUDENT: MY APPLICATIONS
        // ==========================================
        [HttpGet]
        [AuthorizeRole(SessionConstants.RoleStudent)]
        public async Task<IActionResult> MyApplications()
        {
            var studentId = HttpContext.Session.GetCurrentUserId()!.Value;

            var applications = await _context.Applications
                .Include(a => a.PlacementDrive)
                    .ThenInclude(pd => pd!.Company)
                .Where(a => a.StudentId == studentId)
                .OrderByDescending(a => a.ApplicationDate)
                .ToListAsync();

            // Check if student has final placement
            var placement = await _context.Placements
                .Include(p => p.Company)
                .Include(p => p.PlacementDrive)
                .FirstOrDefaultAsync(p => p.StudentId == studentId && p.Status == "Placed");

            ViewBag.Placement = placement;

            return View(applications);
        }

        // ==========================================
        // 2. STUDENT: APPLY FOR A DRIVE
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Apply(int driveId)
        {
            // 1. Check logged in
            if (!HttpContext.Session.IsLoggedIn())
            {
                TempData["Error"] = "Please log in as a student to apply for placement drives.";
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Details", "PlacementDrive", new { id = driveId }) });
            }

            // 2. Check role is student
            if (!HttpContext.Session.IsStudent())
            {
                TempData["Error"] = "Only students are eligible to apply for placement drives.";
                return RedirectToAction("Details", "PlacementDrive", new { id = driveId });
            }

            var studentId = HttpContext.Session.GetCurrentUserId()!.Value;
            var student = await _context.Students.FindAsync(studentId);
            var drive = await _context.PlacementDrives
                .Include(d => d.Company)
                .FirstOrDefaultAsync(d => d.Id == driveId);

            if (student == null || drive == null)
            {
                TempData["Error"] = "Specified student or placement drive was not found.";
                return RedirectToAction("Index", "PlacementDrive");
            }

            // 3. Check duplicate application
            var alreadyApplied = await _context.Applications
                .AnyAsync(a => a.StudentId == studentId && a.PlacementDriveId == driveId);

            if (alreadyApplied)
            {
                TempData["Warning"] = $"You have already applied for '{drive.JobTitle}' at {drive.Company?.CompanyName}.";
                return RedirectToAction("Details", "PlacementDrive", new { id = driveId });
            }

            // 4. Check status & vacancies
            if (drive.Status != "Active")
            {
                TempData["Error"] = $"Cannot apply: This placement drive is currently '{drive.Status}'.";
                return RedirectToAction("Details", "PlacementDrive", new { id = driveId });
            }

            // 5. Check deadline
            if (drive.ApplicationDeadline.Date < DateTime.Today)
            {
                TempData["Error"] = $"Application deadline passed on {drive.ApplicationDeadline:dd MMM yyyy}. Applications are now closed.";
                return RedirectToAction("Details", "PlacementDrive", new { id = driveId });
            }

            // 6. Check Department Eligibility
            bool deptEligible = drive.EligibleDepartment == "All" ||
                string.Equals(drive.EligibleDepartment, student.Department, StringComparison.OrdinalIgnoreCase);

            if (!deptEligible)
            {
                TempData["Error"] = $"Department ineligibility: This drive is open only to '{drive.EligibleDepartment}' students. Your department is '{student.Department}'.";
                return RedirectToAction("Details", "PlacementDrive", new { id = driveId });
            }

            // 7. Check CGPA Eligibility
            if (student.CGPA < drive.EligibilityCGPA)
            {
                TempData["Error"] = $"CGPA ineligibility: Minimum required CGPA is {drive.EligibilityCGPA:F2}. Your current CGPA is {student.CGPA:F2}.";
                return RedirectToAction("Details", "PlacementDrive", new { id = driveId });
            }

            // Create Application Record
            var application = new Application
            {
                StudentId = studentId,
                PlacementDriveId = driveId,
                ApplicationDate = DateTime.Now,
                Status = "Applied",
                Remarks = "Applied directly through student portal."
            };

            _context.Applications.Add(application);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Congratulations! Your application for '{drive.JobTitle}' at {drive.Company?.CompanyName} has been submitted successfully.";
            return RedirectToAction(nameof(MyApplications));
        }

        // ==========================================
        // 3. ADMIN: APPLICATIONS MANAGEMENT
        // ==========================================
        [HttpGet]
        [AuthorizeRole(SessionConstants.RoleAdmin)]
        public async Task<IActionResult> Index(string? searchString, string? status, int? driveId)
        {
            var query = _context.Applications
                .Include(a => a.Student)
                .Include(a => a.PlacementDrive)
                    .ThenInclude(pd => pd!.Company)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var s = searchString.Trim();
                query = query.Where(a => a.Student!.FullName.Contains(s) || 
                                         a.Student.EnrollmentNo.Contains(s) || 
                                         a.PlacementDrive!.JobTitle.Contains(s) || 
                                         a.PlacementDrive.Company!.CompanyName.Contains(s));
            }

            if (!string.IsNullOrWhiteSpace(status) && status != "All")
            {
                query = query.Where(a => a.Status == status);
            }

            if (driveId.HasValue && driveId.Value > 0)
            {
                query = query.Where(a => a.PlacementDriveId == driveId.Value);
            }

            var applications = await query.OrderByDescending(a => a.ApplicationDate).ToListAsync();

            ViewBag.SearchString = searchString;
            ViewBag.SelectedStatus = status;
            ViewBag.SelectedDriveId = driveId;

            ViewBag.Drives = new SelectList(
                await _context.PlacementDrives.Include(d => d.Company)
                    .Select(d => new { d.Id, Title = $"{d.Company!.CompanyName} - {d.JobTitle}" })
                    .ToListAsync(),
                "Id", "Title", driveId);

            return View(applications);
        }

        // GET: Application/Details/5
        [HttpGet]
        [AuthorizeRole(SessionConstants.RoleAdmin)]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var application = await _context.Applications
                .Include(a => a.Student)
                .Include(a => a.PlacementDrive)
                    .ThenInclude(pd => pd!.Company)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (application == null) return NotFound();

            // Check if final placement record exists
            var isPlaced = await _context.Placements
                .AnyAsync(p => p.StudentId == application.StudentId && p.PlacementDriveId == application.PlacementDriveId);

            ViewBag.IsPlaced = isPlaced;

            return View(application);
        }

        // POST: Application/UpdateStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole(SessionConstants.RoleAdmin)]
        public async Task<IActionResult> UpdateStatus(int applicationId, string status, string? remarks)
        {
            var application = await _context.Applications
                .Include(a => a.Student)
                .Include(a => a.PlacementDrive)
                    .ThenInclude(pd => pd!.Company)
                .FirstOrDefaultAsync(a => a.Id == applicationId);

            if (application == null)
            {
                TempData["Error"] = "Application not found.";
                return RedirectToAction(nameof(Index));
            }

            application.Status = status;
            if (!string.IsNullOrWhiteSpace(remarks))
            {
                application.Remarks = remarks.Trim();
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = $"Application status for '{application.Student?.FullName}' updated to '{status}'.";

            if (status == "Selected")
            {
                TempData["Info"] = "Student was Selected! You can now create a final placement record.";
                return RedirectToAction("Create", "Placement", new { applicationId = application.Id });
            }

            return RedirectToAction(nameof(Details), new { id = applicationId });
        }

        // POST: Application/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole(SessionConstants.RoleAdmin)]
        public async Task<IActionResult> Delete(int id)
        {
            var app = await _context.Applications.FindAsync(id);
            if (app != null)
            {
                _context.Applications.Remove(app);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Application record removed.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
