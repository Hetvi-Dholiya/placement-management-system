using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlacementManagementSystem.Data;
using PlacementManagementSystem.Helpers;
using PlacementManagementSystem.Models;
using PlacementManagementSystem.ViewModels;

namespace PlacementManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // If already logged in, provide quick redirect or personalized dashboard link
            var userId = HttpContext.Session.GetCurrentUserId();
            var userRole = HttpContext.Session.GetCurrentUserRole();

            // Calculate live portal statistics
            var totalStudents = await _context.Students.CountAsync();
            var totalCompanies = await _context.Companies.CountAsync();
            var totalDrives = await _context.PlacementDrives.CountAsync();
            var studentsPlaced = await _context.Placements.Where(p => p.Status == "Placed").CountAsync();
            var highestPackage = await _context.Placements.Where(p => p.Status == "Placed").AnyAsync() 
                ? await _context.Placements.Where(p => p.Status == "Placed").MaxAsync(p => p.Package) 
                : 0m;

            // Fetch recent active placement drives
            var activeDrives = await _context.PlacementDrives
                .Include(p => p.Company)
                .Where(p => p.Status == "Active" && p.ApplicationDeadline >= DateTime.Today)
                .OrderBy(p => p.ApplicationDeadline)
                .Take(6)
                .ToListAsync();

            // Recent placements ticker / highlights
            var recentPlacements = await _context.Placements
                .Include(p => p.Student)
                .Include(p => p.Company)
                .Where(p => p.Status == "Placed")
                .OrderByDescending(p => p.PlacementDate)
                .Take(4)
                .ToListAsync();

            ViewBag.TotalStudents = totalStudents;
            ViewBag.TotalCompanies = totalCompanies;
            ViewBag.TotalDrives = totalDrives;
            ViewBag.StudentsPlaced = studentsPlaced;
            ViewBag.HighestPackage = highestPackage;
            ViewBag.RecentPlacements = recentPlacements;

            return View(activeDrives);
        }

        [HttpGet]
        public IActionResult About()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Contact()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
