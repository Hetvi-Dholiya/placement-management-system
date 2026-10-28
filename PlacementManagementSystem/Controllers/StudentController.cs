using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlacementManagementSystem.Data;
using PlacementManagementSystem.Helpers;
using PlacementManagementSystem.Models;
using PlacementManagementSystem.ViewModels;

namespace PlacementManagementSystem.Controllers
{
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public StudentController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // ==========================================
        // 1. DASHBOARD (Dynamic for Admin and Student)
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            if (!HttpContext.Session.IsLoggedIn())
            {
                TempData["Error"] = "Please log in to access the dashboard.";
                return RedirectToAction("Login", "Account");
            }

            if (HttpContext.Session.IsAdmin())
            {
                // ADMIN DASHBOARD
                var totalStudents = await _context.Students.CountAsync();
                var totalCompanies = await _context.Companies.CountAsync();
                var activeDrives = await _context.PlacementDrives.CountAsync(p => p.Status == "Active");
                var totalApps = await _context.Applications.CountAsync();
                var selectedApps = await _context.Applications.CountAsync(a => a.Status == "Selected");
                var placedStudents = await _context.Placements.Where(p => p.Status == "Placed").CountAsync();
                
                var avgPackage = await _context.Placements.Where(p => p.Status == "Placed").AnyAsync()
                    ? await _context.Placements.Where(p => p.Status == "Placed").AverageAsync(p => p.Package)
                    : 0m;

                var highestPackage = await _context.Placements.Where(p => p.Status == "Placed").AnyAsync()
                    ? await _context.Placements.Where(p => p.Status == "Placed").MaxAsync(p => p.Package)
                    : 0m;

                var recentApps = await _context.Applications
                    .Include(a => a.Student)
                    .Include(a => a.PlacementDrive)
                        .ThenInclude(pd => pd!.Company)
                    .OrderByDescending(a => a.ApplicationDate)
                    .Take(5)
                    .ToListAsync();

                var upcomingDrives = await _context.PlacementDrives
                    .Include(pd => pd.Company)
                    .Where(pd => pd.Status == "Active" && pd.DriveDate >= DateTime.Today)
                    .OrderBy(pd => pd.DriveDate)
                    .Take(5)
                    .ToListAsync();

                var recentPlaced = await _context.Placements
                    .Include(p => p.Student)
                    .Include(p => p.Company)
                    .Where(p => p.Status == "Placed")
                    .OrderByDescending(p => p.PlacementDate)
                    .Take(5)
                    .ToListAsync();

                // Status breakdown for metrics
                var statusBreakdown = await _context.Applications
                    .GroupBy(a => a.Status)
                    .Select(g => new { Status = g.Key, Count = g.Count() })
                    .ToDictionaryAsync(x => x.Status, x => x.Count);

                // Department breakdown for metrics
                var deptBreakdown = await _context.Placements
                    .Include(p => p.Student)
                    .Where(p => p.Status == "Placed" && p.Student != null)
                    .GroupBy(p => p.Student!.Department)
                    .Select(g => new { Dept = g.Key, Count = g.Count() })
                    .ToDictionaryAsync(x => x.Dept, x => x.Count);

                var adminVm = new AdminDashboardViewModel
                {
                    TotalStudents = totalStudents,
                    TotalCompanies = totalCompanies,
                    ActivePlacementDrives = activeDrives,
                    TotalApplications = totalApps,
                    TotalSelectedApplications = selectedApps,
                    TotalPlacedStudents = placedStudents,
                    AveragePackage = Math.Round(avgPackage, 2),
                    HighestPackage = highestPackage,
                    RecentApplications = recentApps,
                    UpcomingPlacementDrives = upcomingDrives,
                    RecentlyPlacedStudents = recentPlaced,
                    ApplicationsByStatus = statusBreakdown,
                    PlacementsByDepartment = deptBreakdown
                };

                return View("AdminDashboard", adminVm);
            }
            else
            {
                // STUDENT DASHBOARD
                var studentId = HttpContext.Session.GetCurrentUserId()!.Value;
                var student = await _context.Students.FindAsync(studentId);

                if (student == null)
                {
                    HttpContext.Session.Clear();
                    TempData["Error"] = "Student profile not found. Please log in again.";
                    return RedirectToAction("Login", "Account");
                }

                // Student Stats
                var myApplications = await _context.Applications
                    .Include(a => a.PlacementDrive)
                        .ThenInclude(pd => pd!.Company)
                    .Where(a => a.StudentId == studentId)
                    .OrderByDescending(a => a.ApplicationDate)
                    .ToListAsync();

                var appliedDriveIds = myApplications.Select(a => a.PlacementDriveId).ToHashSet();

                var appliedCount = myApplications.Count;
                var shortlistedCount = myApplications.Count(a => a.Status == "Shortlisted");
                var selectedCount = myApplications.Count(a => a.Status == "Selected");

                var placement = await _context.Placements
                    .Include(p => p.Company)
                    .Include(p => p.PlacementDrive)
                    .FirstOrDefaultAsync(p => p.StudentId == studentId && p.Status == "Placed");

                // Eligible & Active Drives
                var allActiveDrives = await _context.PlacementDrives
                    .Include(pd => pd.Company)
                    .Where(pd => pd.Status == "Active")
                    .OrderBy(pd => pd.ApplicationDeadline)
                    .ToListAsync();

                var driveCards = new List<PlacementDriveCardViewModel>();
                foreach (var d in allActiveDrives)
                {
                    bool isDeptEligible = d.EligibleDepartment == "All" || 
                        string.Equals(d.EligibleDepartment, student.Department, StringComparison.OrdinalIgnoreCase);
                    bool isCgpaEligible = student.CGPA >= d.EligibilityCGPA;
                    bool isDeadlinePassed = d.ApplicationDeadline.Date < DateTime.Today;
                    bool hasApplied = appliedDriveIds.Contains(d.Id);

                    string? ineligibilityReason = null;
                    if (!isDeptEligible)
                        ineligibilityReason = $"Department mismatch (Required: {d.EligibleDepartment})";
                    else if (!isCgpaEligible)
                        ineligibilityReason = $"Minimum CGPA required is {d.EligibilityCGPA:F2} (Your CGPA: {student.CGPA:F2})";
                    else if (isDeadlinePassed)
                        ineligibilityReason = "Application deadline has passed";

                    var app = myApplications.FirstOrDefault(a => a.PlacementDriveId == d.Id);

                    driveCards.Add(new PlacementDriveCardViewModel
                    {
                        Drive = d,
                        IsEligible = isDeptEligible && isCgpaEligible && !isDeadlinePassed,
                        HasApplied = hasApplied,
                        IsDeadlinePassed = isDeadlinePassed,
                        IneligibilityReason = ineligibilityReason,
                        ApplicationStatus = app?.Status
                    });
                }

                var studentVm = new StudentDashboardViewModel
                {
                    Student = student,
                    AvailableDrivesCount = driveCards.Count(d => d.IsEligible && !d.HasApplied),
                    AppliedDrivesCount = appliedCount,
                    ShortlistedCount = shortlistedCount,
                    SelectedCount = selectedCount,
                    IsPlaced = placement != null,
                    FinalPlacement = placement,
                    RecentEligibleDrives = driveCards.Take(4).ToList(),
                    MyRecentApplications = myApplications.Take(5).ToList()
                };

                return View("StudentDashboard", studentVm);
            }
        }

        // ==========================================
        // 2. STUDENT PROFILE (View & Edit)
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            if (!HttpContext.Session.IsLoggedIn())
            {
                TempData["Error"] = "Please log in to view your profile.";
                return RedirectToAction("Login", "Account");
            }

            int studentId;
            if (HttpContext.Session.IsAdmin())
            {
                TempData["Info"] = "Viewing students directory.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                studentId = HttpContext.Session.GetCurrentUserId()!.Value;
            }

            var student = await _context.Students
                .Include(s => s.Applications)
                    .ThenInclude(a => a.PlacementDrive)
                        .ThenInclude(pd => pd!.Company)
                .Include(s => s.Placements)
                    .ThenInclude(p => p.Company)
                .FirstOrDefaultAsync(s => s.Id == studentId);

            if (student == null)
            {
                return NotFound();
            }

            var model = new StudentProfileViewModel
            {
                Id = student.Id,
                EnrollmentNo = student.EnrollmentNo,
                FullName = student.FullName,
                Email = student.Email,
                Phone = student.Phone,
                DateOfBirth = student.DateOfBirth,
                Gender = student.Gender,
                Department = student.Department,
                Semester = student.Semester,
                CGPA = student.CGPA,
                Skills = student.Skills,
                ExistingResumeFileName = student.ResumeFileName,
                ExistingProfilePicture = student.ProfilePicture
            };

            ViewBag.Applications = student.Applications.ToList();
            ViewBag.Placements = student.Placements.ToList();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(StudentProfileViewModel model)
        {
            if (!HttpContext.Session.IsLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            var studentId = HttpContext.Session.GetCurrentUserId()!.Value;
            var student = await _context.Students
                .Include(s => s.Applications)
                    .ThenInclude(a => a.PlacementDrive)
                        .ThenInclude(pd => pd!.Company)
                .Include(s => s.Placements)
                    .ThenInclude(p => p.Company)
                .FirstOrDefaultAsync(s => s.Id == studentId);

            if (student == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                model.EnrollmentNo = student.EnrollmentNo;
                model.ExistingResumeFileName = student.ResumeFileName;
                model.ExistingProfilePicture = student.ProfilePicture;
                ViewBag.Applications = student.Applications.ToList();
                ViewBag.Placements = student.Placements.ToList();
                return View(model);
            }

            // Check if email changed and if new email already exists for another student
            if (!string.Equals(student.Email, model.Email, StringComparison.OrdinalIgnoreCase))
            {
                var emailExists = await _context.Students.AnyAsync(s => s.Email == model.Email && s.Id != studentId);
                if (emailExists)
                {
                    ModelState.AddModelError(nameof(model.Email), "Email is already taken by another student.");
                    model.EnrollmentNo = student.EnrollmentNo;
                    model.ExistingResumeFileName = student.ResumeFileName;
                    model.ExistingProfilePicture = student.ProfilePicture;
                    ViewBag.Applications = student.Applications.ToList();
                    ViewBag.Placements = student.Placements.ToList();
                    return View(model);
                }
            }

            // Handle Profile Picture Upload
            if (model.ProfilePictureFile != null && model.ProfilePictureFile.Length > 0)
            {
                var allowedImgExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
                var imgExtension = Path.GetExtension(model.ProfilePictureFile.FileName).ToLowerInvariant();

                if (!allowedImgExtensions.Contains(imgExtension))
                {
                    ModelState.AddModelError("ProfilePictureFile", "Only image files (.jpg, .jpeg, .png, .webp, .gif) are allowed.");
                    model.EnrollmentNo = student.EnrollmentNo;
                    model.ExistingResumeFileName = student.ResumeFileName;
                    model.ExistingProfilePicture = student.ProfilePicture;
                    ViewBag.Applications = student.Applications.ToList();
                    ViewBag.Placements = student.Placements.ToList();
                    return View(model);
                }

                if (model.ProfilePictureFile.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError("ProfilePictureFile", "Profile picture file size cannot exceed 5MB.");
                    model.EnrollmentNo = student.EnrollmentNo;
                    model.ExistingResumeFileName = student.ResumeFileName;
                    model.ExistingProfilePicture = student.ProfilePicture;
                    ViewBag.Applications = student.Applications.ToList();
                    ViewBag.Placements = student.Placements.ToList();
                    return View(model);
                }

                var imgUploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "profiles");
                if (!Directory.Exists(imgUploadsFolder))
                {
                    Directory.CreateDirectory(imgUploadsFolder);
                }

                var uniqueImgFileName = $"pic_{student.EnrollmentNo}_{Guid.NewGuid().ToString().Substring(0, 8)}{imgExtension}";
                var imgFilePath = Path.Combine(imgUploadsFolder, uniqueImgFileName);

                using (var fileStream = new FileStream(imgFilePath, FileMode.Create))
                {
                    await model.ProfilePictureFile.CopyToAsync(fileStream);
                }

                student.ProfilePicture = uniqueImgFileName;
            }

            // Handle Resume Upload
            if (model.ResumeFile != null && model.ResumeFile.Length > 0)
            {
                var allowedExtensions = new[] { ".pdf", ".docx", ".doc" };
                var extension = Path.GetExtension(model.ResumeFile.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("ResumeFile", "Only PDF and Word documents (.pdf, .docx, .doc) are allowed.");
                    model.EnrollmentNo = student.EnrollmentNo;
                    model.ExistingResumeFileName = student.ResumeFileName;
                    model.ExistingProfilePicture = student.ProfilePicture;
                    ViewBag.Applications = student.Applications.ToList();
                    ViewBag.Placements = student.Placements.ToList();
                    return View(model);
                }

                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "resumes");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = $"{student.EnrollmentNo}_{Guid.NewGuid().ToString().Substring(0, 8)}{extension}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ResumeFile.CopyToAsync(fileStream);
                }

                student.ResumeFileName = uniqueFileName;
            }

            // Update student fields
            student.FullName = model.FullName.Trim();
            student.Email = model.Email.Trim().ToLower();
            student.Phone = model.Phone.Trim();
            student.DateOfBirth = model.DateOfBirth;
            student.Gender = model.Gender;
            student.Department = model.Department;
            student.Semester = model.Semester;
            student.CGPA = model.CGPA;
            student.Skills = model.Skills?.Trim() ?? string.Empty;

            if (!string.IsNullOrWhiteSpace(model.NewPassword))
            {
                student.Password = PasswordHelper.HashPassword(model.NewPassword.Trim());
            }

            await _context.SaveChangesAsync();

            // Refresh session username and profile picture
            HttpContext.Session.SetString(SessionConstants.UserName, student.FullName);
            HttpContext.Session.SetString(SessionConstants.UserEmail, student.Email);
            HttpContext.Session.SetString(SessionConstants.UserProfilePicture, student.ProfilePicture ?? string.Empty);

            TempData["Success"] = "Your profile has been updated successfully.";
            return RedirectToAction(nameof(Profile));
        }

        // ==========================================
        // 3. ADMIN STUDENT MANAGEMENT (CRUD)
        // ==========================================
        [HttpGet]
        [AuthorizeRole(SessionConstants.RoleAdmin)]
        public async Task<IActionResult> Index(string? searchString, string? department, decimal? minCgpa)
        {
            var query = _context.Students
                .Include(s => s.Applications)
                .Include(s => s.Placements)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var search = searchString.Trim();
                query = query.Where(s => s.FullName.Contains(search) || 
                                         s.EnrollmentNo.Contains(search) || 
                                         s.Email.Contains(search) ||
                                         s.Skills.Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(department) && department != "All")
            {
                query = query.Where(s => s.Department == department);
            }

            if (minCgpa.HasValue)
            {
                query = query.Where(s => s.CGPA >= minCgpa.Value);
            }

            var students = await query.OrderBy(s => s.EnrollmentNo).ToListAsync();

            ViewBag.SearchString = searchString;
            ViewBag.SelectedDepartment = department;
            ViewBag.MinCgpa = minCgpa;

            // Distinct departments for filter dropdown
            ViewBag.Departments = await _context.Students
                .Select(s => s.Department)
                .Distinct()
                .OrderBy(d => d)
                .ToListAsync();

            return View(students);
        }

        [HttpGet]
        [AuthorizeRole(SessionConstants.RoleAdmin)]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var student = await _context.Students
                .Include(s => s.Applications)
                    .ThenInclude(a => a.PlacementDrive)
                        .ThenInclude(pd => pd!.Company)
                .Include(s => s.Placements)
                    .ThenInclude(p => p.Company)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (student == null) return NotFound();

            return View(student);
        }

        [HttpGet]
        [AuthorizeRole(SessionConstants.RoleAdmin)]
        public IActionResult Create()
        {
            return View(new Student { DateOfBirth = DateTime.Today.AddYears(-20) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole(SessionConstants.RoleAdmin)]
        public async Task<IActionResult> Create(Student student)
        {
            if (!ModelState.IsValid)
            {
                return View(student);
            }

            // Check uniqueness
            if (await _context.Students.AnyAsync(s => s.EnrollmentNo == student.EnrollmentNo.Trim()))
            {
                ModelState.AddModelError(nameof(student.EnrollmentNo), "Enrollment Number already exists.");
                return View(student);
            }

            if (await _context.Students.AnyAsync(s => s.Email == student.Email.Trim()))
            {
                ModelState.AddModelError(nameof(student.Email), "Email address already registered.");
                return View(student);
            }

            student.Skills = student.Skills?.Trim() ?? string.Empty;
            student.Password = PasswordHelper.HashPassword(student.Password);
            student.CreatedAt = DateTime.Now;

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Student '{student.FullName}' registered successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [AuthorizeRole(SessionConstants.RoleAdmin)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var student = await _context.Students.FindAsync(id);
            if (student == null) return NotFound();

            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole(SessionConstants.RoleAdmin)]
        public async Task<IActionResult> Edit(int id, Student student, string? newPassword)
        {
            if (id != student.Id) return NotFound();

            var existingStudent = await _context.Students.FindAsync(id);
            if (existingStudent == null) return NotFound();

            // Validate duplicate enrollment/email
            if (await _context.Students.AnyAsync(s => s.EnrollmentNo == student.EnrollmentNo && s.Id != id))
            {
                ModelState.AddModelError(nameof(student.EnrollmentNo), "Enrollment Number already in use by another student.");
                return View(student);
            }

            if (await _context.Students.AnyAsync(s => s.Email == student.Email && s.Id != id))
            {
                ModelState.AddModelError(nameof(student.Email), "Email already in use by another student.");
                return View(student);
            }

            existingStudent.EnrollmentNo = student.EnrollmentNo.Trim().ToUpper();
            existingStudent.FullName = student.FullName.Trim();
            existingStudent.Email = student.Email.Trim().ToLower();
            existingStudent.Phone = student.Phone.Trim();
            existingStudent.DateOfBirth = student.DateOfBirth;
            existingStudent.Gender = student.Gender;
            existingStudent.Department = student.Department;
            existingStudent.Semester = student.Semester;
            existingStudent.CGPA = student.CGPA;
            existingStudent.Skills = student.Skills?.Trim() ?? string.Empty;

            if (!string.IsNullOrWhiteSpace(newPassword))
            {
                existingStudent.Password = PasswordHelper.HashPassword(newPassword.Trim());
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = $"Student record for '{existingStudent.FullName}' updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [AuthorizeRole(SessionConstants.RoleAdmin)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var student = await _context.Students
                .Include(s => s.Applications)
                .Include(s => s.Placements)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (student == null) return NotFound();

            return View(student);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuthorizeRole(SessionConstants.RoleAdmin)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Students
                .Include(s => s.Placements)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (student == null) return NotFound();

            // Check if student has final placement records
            if (student.Placements.Any())
            {
                TempData["Error"] = "Cannot delete student because active placement records are linked to this student. Please remove the placement record first.";
                return RedirectToAction(nameof(Details), new { id });
            }

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Student record for '{student.FullName}' has been deleted.";
            return RedirectToAction(nameof(Index));
        }
    }
}
