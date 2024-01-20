using Microsoft.AspNetCore.Mvc;
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

        public async Task<IActionResult> Index()
        {
            var companies = await _context.Companies.ToListAsync();

            if (companies.Count == 0)
            {
                ViewBag.Message = "No Companies have Registered";
                return View();
            }
            return View(companies);
        }

        public async Task<IActionResult> Edit(int CompanyId)
        {
            var registeredCompany = await _context.Companies
                .Where(c => c.CompanyId == CompanyId)
                .Select(c => new CompanyDetailsViewModel
                {
                    CompanyName = c.CompanyName,
                    CompanyPhoneNumber = c.CompanyPhoneNumber,
                    CompanyEmail = c.CompanyEmail,
                    //LicenceName = organisation.Licences.FirstOrDefault()?.Name ?? "DefaultLicenceName",
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
    }
}
