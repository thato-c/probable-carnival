using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProductManager.Data;
using ProductManager.ViewModels;

namespace ProductManager.Controllers
{
    public class CompanyController : Controller
    {
        private readonly ApplicationDBContext _context;

        public CompanyController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var companies = await _context.Companies.ToListAsync();

                if (companies.Count == 0)
                {
                    ViewBag.Message = "No Companies have Registered";
                    return View();
                }
                return View(companies);
            }
            catch (DbUpdateException ex)
            {
                // Log the exception details
                Console.WriteLine($"DbUpdateException: {ex.Message}");
                Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");

                // Optionally, log additional details
                // Log the SQL statement causing the exception
                Console.WriteLine($"SQL: {ex.InnerException?.InnerException?.Message}");
                ModelState.AddModelError("", "An error occurred while retrieving data from the database.");

                ViewBag.Message = "An error occurred while retrieving data from the database.";
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var licences = await _context.Licences
                .Select(l => new SelectListItem
                {
                    Value  = l.LicenceId.ToString(),
                    Text = l.Name,
                }).ToListAsync();

            ViewBag.Licences = new SelectList(licences, "Value", "Text");

            var viewModel = new CompanyDetailsViewModel();

            viewModel.Quantity = 1;
            viewModel.PurchaseDate = DateTime.Now;

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int CompanyId)
        {
            try
            {
                var registeredCompany = await _context.Companies
                .Where(c => c.CompanyId == CompanyId)
                .Select(c => new CompanyDetailsViewModel
                {
                    CompanyName = c.CompanyName,
                    CompanyPhoneNumber = c.CompanyPhoneNumber,
                    CompanyEmail = c.CompanyEmail,
                    LicenceName = c.LicencePurchases
                        .Join(_context.Licences, p => p.LicenceId, l => l.LicenceId, (p, l) => l.Name)
                        .FirstOrDefault() ?? "Default Licence Name",
                    AdminEmail = c.Users
                        .Select(u => u.Username)
                        .FirstOrDefault() ?? "Deafult Admin Email",
                    Quantity = c.LicencePurchases.Sum(purchase => (int?)purchase.Quantity) ?? 0,
                    TotalCost = c.LicencePurchases.Sum(purchase => (decimal?)purchase.TotalCost) ?? 0,
                })
                .FirstOrDefaultAsync();

                if (registeredCompany != null)
                {
                    return View(registeredCompany);
                }

                ViewBag.Message = "The Company does not exist";
                return View();
            }
            catch (DbUpdateException ex)
            {
                // Log the exception details
                Console.WriteLine($"DbUpdateException: {ex.Message}");
                Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");

                // Optionally, log additional details
                // Log the SQL statement causing the exception
                Console.WriteLine($"SQL: {ex.InnerException?.InnerException?.Message}");
                ModelState.AddModelError("", "An error occurred while retrieving data from the database.");
                return View();
            }
            
        }
    }
}
