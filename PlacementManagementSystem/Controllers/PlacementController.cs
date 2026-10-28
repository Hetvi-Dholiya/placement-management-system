using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PlacementManagementSystem.Data;
using PlacementManagementSystem.Helpers;
using PlacementManagementSystem.Models;
using PlacementManagementSystem.ViewModels;

namespace PlacementManagementSystem.Controllers
{
    public class PlacementController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PlacementController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // 1. PLACEMENTS LISTING (Admin & Public/Student)
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> Index(string? searchString, int? companyId, string? department)
        {
            var query = _context.Placements
                .Include(p => p.Student)
                .Include(p => p.Company)
                .Include(p => p.PlacementDrive)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var s = searchString.Trim();
                query = query.Where(p => p.Student!.FullName.Contains(s) || 
                                         p.Student.EnrollmentNo.Contains(s) || 
                                         p.Company!.CompanyName.Contains(s) || 
                                         p.JobRole.Contains(s));
            }

            if (companyId.HasValue && companyId.Value > 0)
            {
                query = query.Where(p => p.CompanyId == companyId.Value);
            }

            if (!string.IsNullOrWhiteSpace(department) && department != "All")
            {
                query = query.Where(p => p.Student!.Department == department);
            }

            var placements = await query.OrderByDescending(p => p.PlacementDate).ToListAsync();

            // Placement Statistics
            var totalPlaced = placements.Count(p => p.Status == "Placed");
            var avgPackage = totalPlaced > 0 ? placements.Where(p => p.Status == "Placed").Average(p => p.Package) : 0m;
            var maxPackage = totalPlaced > 0 ? placements.Where(p => p.Status == "Placed").Max(p => p.Package) : 0m;

            ViewBag.TotalPlaced = totalPlaced;
            ViewBag.AvgPackage = Math.Round(avgPackage, 2);
            ViewBag.MaxPackage = maxPackage;
            ViewBag.SearchString = searchString;
            ViewBag.SelectedCompanyId = companyId;
            ViewBag.SelectedDepartment = department;

            ViewBag.Companies = new SelectList(await _context.Companies.OrderBy(c => c.CompanyName).ToListAsync(), "Id", "CompanyName", companyId);
            ViewBag.Departments = await _context.Students.Select(s => s.Department).Distinct().OrderBy(d => d).ToListAsync();

            return View(placements);
        }

        // ==========================================
        // 2. STUDENT: MY PLACEMENT STATUS
        // ==========================================
        [HttpGet]
        [AuthorizeRole(SessionConstants.RoleStudent)]
        public async Task<IActionResult> MyPlacement()
        {
            var studentId = HttpContext.Session.GetCurrentUserId()!.Value;

            var placement = await _context.Placements
                .Include(p => p.Student)
                .Include(p => p.Company)
                .Include(p => p.PlacementDrive)
                .FirstOrDefaultAsync(p => p.StudentId == studentId);

            return View(placement);
        }

        // ==========================================
        // 3. ADMIN: DETAILS
        // ==========================================
        [HttpGet]
        [AuthorizeRole(SessionConstants.RoleAdmin)]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var placement = await _context.Placements
                .Include(p => p.Student)
                .Include(p => p.Company)
                .Include(p => p.PlacementDrive)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (placement == null) return NotFound();

            return View(placement);
        }

        // ==========================================
        // 4. ADMIN: CREATE PLACEMENT RECORD
        // ==========================================
        [HttpGet]
        [AuthorizeRole(SessionConstants.RoleAdmin)]
        public async Task<IActionResult> Create(int? applicationId)
        {
            var model = new PlacementCreateViewModel
            {
                PlacementDate = DateTime.Today,
                Status = "Placed"
            };

            if (applicationId.HasValue)
            {
                var app = await _context.Applications
                    .Include(a => a.Student)
                    .Include(a => a.PlacementDrive)
                        .ThenInclude(pd => pd!.Company)
                    .FirstOrDefaultAsync(a => a.Id == applicationId.Value);

                if (app != null)
                {
                    model.ApplicationId = app.Id;
                    model.StudentId = app.StudentId;
                    model.CompanyId = app.PlacementDrive!.CompanyId;
                    model.PlacementDriveId = app.PlacementDriveId;
                    model.Package = app.PlacementDrive.SalaryPackage;
                    model.JobRole = app.PlacementDrive.JobTitle;

                    ViewBag.SelectedStudentName = $"{app.Student?.FullName} ({app.Student?.EnrollmentNo})";
                    ViewBag.SelectedCompanyName = app.PlacementDrive.Company?.CompanyName;
                    ViewBag.SelectedDriveTitle = app.PlacementDrive.JobTitle;
                }
            }

            await PopulateDropdowns(model.StudentId, model.CompanyId, model.PlacementDriveId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole(SessionConstants.RoleAdmin)]
        public async Task<IActionResult> Create(PlacementCreateViewModel model)
        {
            // Check if this student already has a placement for this drive
            var exists = await _context.Placements
                .AnyAsync(p => p.StudentId == model.StudentId && p.PlacementDriveId == model.PlacementDriveId);

            if (exists)
            {
                ModelState.AddModelError(string.Empty, "A placement record already exists for this student and placement drive.");
            }

            if (!ModelState.IsValid)
            {
                await PopulateDropdowns(model.StudentId, model.CompanyId, model.PlacementDriveId);
                return View(model);
            }

            var placement = new Placement
            {
                StudentId = model.StudentId,
                CompanyId = model.CompanyId,
                PlacementDriveId = model.PlacementDriveId,
                PlacementDate = model.PlacementDate,
                Package = model.Package,
                JobRole = model.JobRole.Trim(),
                Status = model.Status
            };

            _context.Placements.Add(placement);

            // Also ensure the matching application status is set to Selected
            var app = await _context.Applications
                .FirstOrDefaultAsync(a => a.StudentId == model.StudentId && a.PlacementDriveId == model.PlacementDriveId);
            if (app != null && app.Status != "Selected")
            {
                app.Status = "Selected";
                app.Remarks = $"Offer received: {model.JobRole} with package {model.Package:F2} LPA.";
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Official placement record created successfully.";
            return RedirectToAction(nameof(Index));
        }

        // ==========================================
        // 5. ADMIN: EDIT PLACEMENT RECORD
        // ==========================================
        [HttpGet]
        [AuthorizeRole(SessionConstants.RoleAdmin)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var placement = await _context.Placements
                .Include(p => p.Student)
                .Include(p => p.Company)
                .Include(p => p.PlacementDrive)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (placement == null) return NotFound();

            await PopulateDropdowns(placement.StudentId, placement.CompanyId, placement.PlacementDriveId);
            return View(placement);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole(SessionConstants.RoleAdmin)]
        public async Task<IActionResult> Edit(int id, Placement placement)
        {
            if (id != placement.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                await PopulateDropdowns(placement.StudentId, placement.CompanyId, placement.PlacementDriveId);
                return View(placement);
            }

            var existing = await _context.Placements.FindAsync(id);
            if (existing == null) return NotFound();

            existing.StudentId = placement.StudentId;
            existing.CompanyId = placement.CompanyId;
            existing.PlacementDriveId = placement.PlacementDriveId;
            existing.PlacementDate = placement.PlacementDate;
            existing.Package = placement.Package;
            existing.JobRole = placement.JobRole.Trim();
            existing.Status = placement.Status;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Placement record updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        // ==========================================
        // 6. ADMIN: DELETE PLACEMENT RECORD
        // ==========================================
        [HttpGet]
        [AuthorizeRole(SessionConstants.RoleAdmin)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var placement = await _context.Placements
                .Include(p => p.Student)
                .Include(p => p.Company)
                .Include(p => p.PlacementDrive)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (placement == null) return NotFound();

            return View(placement);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuthorizeRole(SessionConstants.RoleAdmin)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var placement = await _context.Placements.FindAsync(id);
            if (placement != null)
            {
                _context.Placements.Remove(placement);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Placement record deleted.";
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateDropdowns(int selectedStudentId = 0, int selectedCompanyId = 0, int selectedDriveId = 0)
        {
            ViewBag.StudentId = new SelectList(
                await _context.Students.OrderBy(s => s.FullName)
                    .Select(s => new { s.Id, Display = $"{s.FullName} ({s.EnrollmentNo} - {s.Department})" })
                    .ToListAsync(),
                "Id", "Display", selectedStudentId);

            ViewBag.CompanyId = new SelectList(
                await _context.Companies.OrderBy(c => c.CompanyName).ToListAsync(),
                "Id", "CompanyName", selectedCompanyId);

            ViewBag.PlacementDriveId = new SelectList(
                await _context.PlacementDrives.Include(d => d.Company).OrderByDescending(d => d.DriveDate)
                    .Select(d => new { d.Id, Display = $"{d.Company!.CompanyName} - {d.JobTitle}" })
                    .ToListAsync(),
                "Id", "Display", selectedDriveId);
        }
    }
}
