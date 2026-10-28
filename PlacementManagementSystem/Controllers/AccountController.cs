using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlacementManagementSystem.Data;
using PlacementManagementSystem.Helpers;
using PlacementManagementSystem.Models;
using PlacementManagementSystem.ViewModels;

namespace PlacementManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (HttpContext.Session.IsLoggedIn())
            {
                if (HttpContext.Session.IsAdmin())
                    return RedirectToAction("Dashboard", "Student"); // Admin dashboard
                return RedirectToAction("Dashboard", "Student"); // Student dashboard
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var username = model.Username.Trim();
            var password = model.Password;

            // 1. Check if user is Admin
            var admin = await _context.AdminUsers
                .FirstOrDefaultAsync(a => a.Email.ToLower() == username.ToLower());

            if (admin != null && PasswordHelper.VerifyPassword(password, admin.Password))
            {
                // Set Session State
                HttpContext.Session.SetInt32(SessionConstants.UserId, admin.Id);
                HttpContext.Session.SetString(SessionConstants.UserName, admin.FullName);
                HttpContext.Session.SetString(SessionConstants.UserEmail, admin.Email);
                HttpContext.Session.SetString(SessionConstants.UserRole, SessionConstants.RoleAdmin);

                TempData["Success"] = $"Welcome, {admin.FullName}! Logged in as Placement Officer.";

                if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                {
                    return Redirect(model.ReturnUrl);
                }
                return RedirectToAction("Dashboard", "Student");
            }

            // 2. Check if user is Student (by Email or EnrollmentNo)
            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.Email.ToLower() == username.ToLower() || s.EnrollmentNo.ToLower() == username.ToLower());

            if (student != null && PasswordHelper.VerifyPassword(password, student.Password))
            {
                // Set Session State
                HttpContext.Session.SetInt32(SessionConstants.UserId, student.Id);
                HttpContext.Session.SetString(SessionConstants.UserName, student.FullName);
                HttpContext.Session.SetString(SessionConstants.UserEmail, student.Email);
                HttpContext.Session.SetString(SessionConstants.UserRole, SessionConstants.RoleStudent);
                HttpContext.Session.SetString(SessionConstants.UserProfilePicture, student.ProfilePicture ?? string.Empty);

                TempData["Success"] = $"Welcome back, {student.FullName}!";

                if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                {
                    return Redirect(model.ReturnUrl);
                }
                return RedirectToAction("Dashboard", "Student");
            }

            // Invalid credentials
            ModelState.AddModelError(string.Empty, "Invalid login credentials. Please check your Email / Enrollment Number and password.");
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (HttpContext.Session.IsLoggedIn())
            {
                return RedirectToAction("Dashboard", "Student");
            }

            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Check duplicate enrollment number
            var enrollmentExists = await _context.Students
                .AnyAsync(s => s.EnrollmentNo.ToLower() == model.EnrollmentNo.Trim().ToLower());
            if (enrollmentExists)
            {
                ModelState.AddModelError(nameof(model.EnrollmentNo), "A student with this Enrollment Number already exists.");
                return View(model);
            }

            // Check duplicate email
            var emailExists = await _context.Students
                .AnyAsync(s => s.Email.ToLower() == model.Email.Trim().ToLower());
            if (emailExists)
            {
                ModelState.AddModelError(nameof(model.Email), "A student with this Email Address already exists.");
                return View(model);
            }

            // Map and Save Student
            var student = new Student
            {
                EnrollmentNo = model.EnrollmentNo.Trim().ToUpper(),
                FullName = model.FullName.Trim(),
                Email = model.Email.Trim().ToLower(),
                Phone = model.Phone.Trim(),
                DateOfBirth = model.DateOfBirth,
                Gender = model.Gender,
                Department = model.Department,
                Semester = model.Semester,
                CGPA = model.CGPA,
                Skills = model.Skills?.Trim() ?? string.Empty,
                Password = PasswordHelper.HashPassword(model.Password),
                Role = "Student",
                CreatedAt = DateTime.Now
            };

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Registration successful! You can now log in using your Enrollment Number or Email.";
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["Success"] = "You have been logged out successfully.";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
