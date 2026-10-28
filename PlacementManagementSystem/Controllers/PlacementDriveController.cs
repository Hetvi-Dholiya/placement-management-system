using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PlacementManagementSystem.Data;
using PlacementManagementSystem.Helpers;
using PlacementManagementSystem.Models;
using PlacementManagementSystem.ViewModels;

namespace PlacementManagementSystem.Controllers
{
    public class PlacementDriveController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PlacementDriveController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: PlacementDrive
        [HttpGet]
        public async Task<IActionResult> Index(string? searchString, int? companyId, string? department, decimal? maxMinCgpa, string? status)
        {
            var query = _context.PlacementDrives
                .Include(pd => pd.Company)
                .Include(pd => pd.Applications)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var s = searchString.Trim();
                query = query.Where(pd => pd.JobTitle.Contains(s) || 
                                          pd.Company!.CompanyName.Contains(s) || 
                                          pd.JobLocation.Contains(s) || 
                                          pd.RequiredSkills.Contains(s));
            }

            if (companyId.HasValue && companyId.Value > 0)
            {
                query = query.Where(pd => pd.CompanyId == companyId.Value);
            }

            if (!string.IsNullOrWhiteSpace(department) && department != "All")
            {
                query = query.Where(pd => pd.EligibleDepartment == "All" || pd.EligibleDepartment == department);
            }

            if (maxMinCgpa.HasValue)
            {
                query = query.Where(pd => pd.EligibilityCGPA <= maxMinCgpa.Value);
            }

            if (!string.IsNullOrWhiteSpace(status) && status != "All")
            {
                query = query.Where(pd => pd.Status == status);
            }

            var drives = await query.OrderByDescending(pd => pd.CreatedAt).ToListAsync();

            // Check student context if logged in
            Student? currentStudent = null;
            var appliedDriveIds = new HashSet<int>();
            var applicationStatusMap = new Dictionary<int, string>();

            if (HttpContext.Session.IsLoggedIn() && HttpContext.Session.IsStudent())
            {
                var studentId = HttpContext.Session.GetCurrentUserId()!.Value;
                currentStudent = await _context.Students.FindAsync(studentId);

                var apps = await _context.Applications
                    .Where(a => a.StudentId == studentId)
                    .ToListAsync();

                foreach (var a in apps)
                {
                    appliedDriveIds.Add(a.PlacementDriveId);
                    applicationStatusMap[a.PlacementDriveId] = a.Status;
                }
            }

            var driveCardList = new List<PlacementDriveCardViewModel>();
            foreach (var d in drives)
            {
                bool isEligible = true;
                bool hasApplied = appliedDriveIds.Contains(d.Id);
                bool isDeadlinePassed = d.ApplicationDeadline.Date < DateTime.Today;
                string? reason = null;

                if (currentStudent != null)
                {
                    bool deptMatches = d.EligibleDepartment == "All" || 
                        string.Equals(d.EligibleDepartment, currentStudent.Department, StringComparison.OrdinalIgnoreCase);
                    bool cgpaMatches = currentStudent.CGPA >= d.EligibilityCGPA;

                    if (!deptMatches)
                    {
                        isEligible = false;
                        reason = $"Department requirement: {d.EligibleDepartment}";
                    }
                    else if (!cgpaMatches)
                    {
                        isEligible = false;
                        reason = $"Requires min CGPA {d.EligibilityCGPA:F2} (Your CGPA: {currentStudent.CGPA:F2})";
                    }
                    else if (isDeadlinePassed)
                    {
                        isEligible = false;
                        reason = "Deadline passed";
                    }
                    else if (d.Status != "Active")
                    {
                        isEligible = false;
                        reason = $"Drive is {d.Status}";
                    }
                }
                else
                {
                    // Guest or Admin
                    if (isDeadlinePassed) isEligible = false;
                }

                driveCardList.Add(new PlacementDriveCardViewModel
                {
                    Drive = d,
                    IsEligible = isEligible,
                    HasApplied = hasApplied,
                    IsDeadlinePassed = isDeadlinePassed,
                    IneligibilityReason = reason,
                    ApplicationStatus = applicationStatusMap.ContainsKey(d.Id) ? applicationStatusMap[d.Id] : null
                });
            }

            ViewBag.SearchString = searchString;
            ViewBag.SelectedCompanyId = companyId;
            ViewBag.SelectedDepartment = department;
            ViewBag.SelectedStatus = status;

            ViewBag.Companies = new SelectList(await _context.Companies.OrderBy(c => c.CompanyName).ToListAsync(), "Id", "CompanyName", companyId);
            ViewBag.Departments = await _context.PlacementDrives
                .Select(pd => pd.EligibleDepartment)
                .Distinct()
                .OrderBy(d => d)
                .ToListAsync();

            return View(driveCardList);
        }

        // GET: PlacementDrive/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var drive = await _context.PlacementDrives
                .Include(pd => pd.Company)
                .Include(pd => pd.Applications)
                    .ThenInclude(a => a.Student)
                .Include(pd => pd.Placements)
                .FirstOrDefaultAsync(pd => pd.Id == id);

            if (drive == null) return NotFound();

            // Evaluate eligibility for logged-in student
            Student? currentStudent = null;
            bool isEligible = true;
            bool hasApplied = false;
            string? applicationStatus = null;
            string? ineligibilityReason = null;
            bool deadlinePassed = drive.ApplicationDeadline.Date < DateTime.Today;

            if (HttpContext.Session.IsLoggedIn() && HttpContext.Session.IsStudent())
            {
                var studentId = HttpContext.Session.GetCurrentUserId()!.Value;
                currentStudent = await _context.Students.FindAsync(studentId);

                var existingApp = await _context.Applications
                    .FirstOrDefaultAsync(a => a.StudentId == studentId && a.PlacementDriveId == drive.Id);

                if (existingApp != null)
                {
                    hasApplied = true;
                    applicationStatus = existingApp.Status;
                }

                if (currentStudent != null)
                {
                    bool deptMatches = drive.EligibleDepartment == "All" || 
                        string.Equals(drive.EligibleDepartment, currentStudent.Department, StringComparison.OrdinalIgnoreCase);
                    bool cgpaMatches = currentStudent.CGPA >= drive.EligibilityCGPA;

                    if (!deptMatches)
                    {
                        isEligible = false;
                        ineligibilityReason = $"Your department ({currentStudent.Department}) is not eligible for this role. Eligible: {drive.EligibleDepartment}.";
                    }
                    else if (!cgpaMatches)
                    {
                        isEligible = false;
                        ineligibilityReason = $"Your CGPA ({currentStudent.CGPA:F2}) is below the minimum required CGPA of {drive.EligibilityCGPA:F2}.";
                    }
                    else if (deadlinePassed)
                    {
                        isEligible = false;
                        ineligibilityReason = $"The application deadline ({drive.ApplicationDeadline:dd MMM yyyy}) has passed.";
                    }
                    else if (drive.Status != "Active")
                    {
                        isEligible = false;
                        ineligibilityReason = $"This placement drive is currently {drive.Status}.";
                    }
                }
            }

            ViewBag.CurrentStudent = currentStudent;
            ViewBag.IsEligible = isEligible;
            ViewBag.HasApplied = hasApplied;
            ViewBag.ApplicationStatus = applicationStatus;
            ViewBag.IneligibilityReason = ineligibilityReason;
            ViewBag.DeadlinePassed = deadlinePassed;

            return View(drive);
        }

        // GET: PlacementDrive/Create
        [HttpGet]
        [AuthorizeRole(SessionConstants.RoleAdmin)]
        public async Task<IActionResult> Create()
        {
            ViewBag.CompanyId = new SelectList(await _context.Companies.OrderBy(c => c.CompanyName).ToListAsync(), "Id", "CompanyName");
            return View(new PlacementDrive
            {
                DriveDate = DateTime.Today.AddDays(14),
                ApplicationDeadline = DateTime.Today.AddDays(7),
                EligibilityCGPA = 7.00m,
                EligibleDepartment = "All",
                NumberOfVacancies = 5,
                Status = "Active"
            });
        }

        // POST: PlacementDrive/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole(SessionConstants.RoleAdmin)]
        public async Task<IActionResult> Create(PlacementDrive placementDrive)
        {
            if (placementDrive.ApplicationDeadline > placementDrive.DriveDate)
            {
                ModelState.AddModelError(nameof(placementDrive.ApplicationDeadline), "Application deadline cannot be after the drive date.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.CompanyId = new SelectList(await _context.Companies.OrderBy(c => c.CompanyName).ToListAsync(), "Id", "CompanyName", placementDrive.CompanyId);
                return View(placementDrive);
            }

            placementDrive.CreatedAt = DateTime.Now;
            _context.PlacementDrives.Add(placementDrive);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Placement Drive for '{placementDrive.JobTitle}' created successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: PlacementDrive/Edit/5
        [HttpGet]
        [AuthorizeRole(SessionConstants.RoleAdmin)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var drive = await _context.PlacementDrives.FindAsync(id);
            if (drive == null) return NotFound();

            ViewBag.CompanyId = new SelectList(await _context.Companies.OrderBy(c => c.CompanyName).ToListAsync(), "Id", "CompanyName", drive.CompanyId);
            return View(drive);
        }

        // POST: PlacementDrive/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole(SessionConstants.RoleAdmin)]
        public async Task<IActionResult> Edit(int id, PlacementDrive placementDrive)
        {
            if (id != placementDrive.Id) return NotFound();

            if (placementDrive.ApplicationDeadline > placementDrive.DriveDate)
            {
                ModelState.AddModelError(nameof(placementDrive.ApplicationDeadline), "Application deadline cannot be after the drive date.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.CompanyId = new SelectList(await _context.Companies.OrderBy(c => c.CompanyName).ToListAsync(), "Id", "CompanyName", placementDrive.CompanyId);
                return View(placementDrive);
            }

            var existing = await _context.PlacementDrives.FindAsync(id);
            if (existing == null) return NotFound();

            existing.CompanyId = placementDrive.CompanyId;
            existing.JobTitle = placementDrive.JobTitle.Trim();
            existing.JobDescription = placementDrive.JobDescription.Trim();
            existing.EligibilityCGPA = placementDrive.EligibilityCGPA;
            existing.EligibleDepartment = placementDrive.EligibleDepartment.Trim();
            existing.RequiredSkills = placementDrive.RequiredSkills.Trim();
            existing.SalaryPackage = placementDrive.SalaryPackage;
            existing.JobLocation = placementDrive.JobLocation.Trim();
            existing.DriveDate = placementDrive.DriveDate;
            existing.ApplicationDeadline = placementDrive.ApplicationDeadline;
            existing.NumberOfVacancies = placementDrive.NumberOfVacancies;
            existing.Status = placementDrive.Status;

            await _context.SaveChangesAsync();

            TempData["Success"] = $"Placement Drive '{existing.JobTitle}' updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: PlacementDrive/Delete/5
        [HttpGet]
        [AuthorizeRole(SessionConstants.RoleAdmin)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var drive = await _context.PlacementDrives
                .Include(pd => pd.Company)
                .Include(pd => pd.Applications)
                .Include(pd => pd.Placements)
                .FirstOrDefaultAsync(pd => pd.Id == id);

            if (drive == null) return NotFound();

            return View(drive);
        }

        // POST: PlacementDrive/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuthorizeRole(SessionConstants.RoleAdmin)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var drive = await _context.PlacementDrives
                .Include(pd => pd.Placements)
                .FirstOrDefaultAsync(pd => pd.Id == id);

            if (drive == null) return NotFound();

            if (drive.Placements.Any())
            {
                TempData["Error"] = $"Cannot delete placement drive '{drive.JobTitle}' because students have already been officially placed from this drive.";
                return RedirectToAction(nameof(Details), new { id });
            }

            _context.PlacementDrives.Remove(drive);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Placement Drive '{drive.JobTitle}' deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
