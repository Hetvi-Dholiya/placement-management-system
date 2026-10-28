using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlacementManagementSystem.Data;
using PlacementManagementSystem.Helpers;
using PlacementManagementSystem.Models;

namespace PlacementManagementSystem.Controllers
{
    public class CompanyController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CompanyController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Company
        [HttpGet]
        public async Task<IActionResult> Index(string? searchString, string? industry)
        {
            var query = _context.Companies
                .Include(c => c.PlacementDrives)
                .Include(c => c.Placements)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var search = searchString.Trim();
                query = query.Where(c => c.CompanyName.Contains(search) || 
                                         c.Location.Contains(search) || 
                                         c.HRName.Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(industry) && industry != "All")
            {
                query = query.Where(c => c.Industry == industry);
            }

            var companies = await query.OrderBy(c => c.CompanyName).ToListAsync();

            ViewBag.SearchString = searchString;
            ViewBag.SelectedIndustry = industry;
            ViewBag.Industries = await _context.Companies
                .Select(c => c.Industry)
                .Distinct()
                .OrderBy(i => i)
                .ToListAsync();

            return View(companies);
        }

        // GET: Company/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var company = await _context.Companies
                .Include(c => c.PlacementDrives)
                    .ThenInclude(pd => pd.Applications)
                .Include(c => c.Placements)
                    .ThenInclude(p => p.Student)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (company == null) return NotFound();

            return View(company);
        }

        // GET: Company/Create
        [HttpGet]
        [AuthorizeRole(SessionConstants.RoleAdmin)]
        public IActionResult Create()
        {
            return View(new Company());
        }

        // POST: Company/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole(SessionConstants.RoleAdmin)]
        public async Task<IActionResult> Create(Company company)
        {
            if (!ModelState.IsValid)
            {
                return View(company);
            }

            if (await _context.Companies.AnyAsync(c => c.CompanyName.ToLower() == company.CompanyName.Trim().ToLower()))
            {
                ModelState.AddModelError(nameof(company.CompanyName), "A company with this name is already registered.");
                return View(company);
            }

            company.CreatedAt = DateTime.Now;
            _context.Companies.Add(company);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Company '{company.CompanyName}' was added successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Company/Edit/5
        [HttpGet]
        [AuthorizeRole(SessionConstants.RoleAdmin)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var company = await _context.Companies.FindAsync(id);
            if (company == null) return NotFound();

            return View(company);
        }

        // POST: Company/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole(SessionConstants.RoleAdmin)]
        public async Task<IActionResult> Edit(int id, Company company)
        {
            if (id != company.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                return View(company);
            }

            var existingCompany = await _context.Companies.FindAsync(id);
            if (existingCompany == null) return NotFound();

            if (await _context.Companies.AnyAsync(c => c.CompanyName.ToLower() == company.CompanyName.Trim().ToLower() && c.Id != id))
            {
                ModelState.AddModelError(nameof(company.CompanyName), "Another company with this name already exists.");
                return View(company);
            }

            existingCompany.CompanyName = company.CompanyName.Trim();
            existingCompany.CompanyDescription = company.CompanyDescription.Trim();
            existingCompany.Industry = company.Industry.Trim();
            existingCompany.Location = company.Location.Trim();
            existingCompany.Website = company.Website?.Trim();
            existingCompany.HRName = company.HRName.Trim();
            existingCompany.HREmail = company.HREmail.Trim();
            existingCompany.ContactNumber = company.ContactNumber.Trim();

            await _context.SaveChangesAsync();

            TempData["Success"] = $"Company '{existingCompany.CompanyName}' details updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Company/Delete/5
        [HttpGet]
        [AuthorizeRole(SessionConstants.RoleAdmin)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var company = await _context.Companies
                .Include(c => c.PlacementDrives)
                .Include(c => c.Placements)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (company == null) return NotFound();

            return View(company);
        }

        // POST: Company/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuthorizeRole(SessionConstants.RoleAdmin)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var company = await _context.Companies
                .Include(c => c.PlacementDrives)
                .Include(c => c.Placements)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (company == null) return NotFound();

            if (company.Placements.Any())
            {
                TempData["Error"] = $"Cannot delete company '{company.CompanyName}' because students have already been placed in this company.";
                return RedirectToAction(nameof(Details), new { id });
            }

            _context.Companies.Remove(company);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Company '{company.CompanyName}' and its associated drives have been removed.";
            return RedirectToAction(nameof(Index));
        }
    }
}
