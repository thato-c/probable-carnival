using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging.Licenses;
using ProductManager.Data;
using ProductManager.Models;
using ProductManager.ViewModels;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ProductManager.Controllers
{
    public class LicencePurchaseController : Controller
    {

        private readonly ApplicationDBContext _context;

        public LicencePurchaseController(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new LicencePurchaseViewModel();

            await SetDefaultLicence(viewModel);
            return View(viewModel);
        }

        public async Task<IActionResult> Create(LicencePurchaseViewModel model) 
        {
            if (ModelState.IsValid)
            {
                // Map viewModel to Company entity
                var Company = new Models.Company
                {
                    CompanyName = model.CompanyDetails.Name,
                    CompanyEmail = model.CompanyDetails.Email,
                    CompanyPhoneNumber = model.CompanyDetails.PhoneNumber,
                };

                try
                {
                    // Add and save the new licence to the database
                    _context.Companies.Add(Company);
                    await _context.SaveChangesAsync();

                    // Map the viewModel to LicencePurchase entity
                    var licenceId = await GetLicenceId(model.LicenceName);
                    var LicencePurchase = new Models.LicencePurchase
                    {
                        Quantity = model.Quantity,
                        PurchaseDate = model.PurchaseDate,
                        TotalCost = model.TotalCost,
                        CompanyId = Company.CompanyId,
                        LicenceId = licenceId,
                    };

                    // Add and save the new licence purchase to the database
                    _context.LicencePurchases.Add(LicencePurchase);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Index");
                }
                catch (DbUpdateException ex)
                {
                    // Log the exception details
                    Console.WriteLine($"DbUpdateException: {ex.Message}");
                    Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");

                    // Optionally, log additional details
                    // Log the SQL statement causing the exception
                    Console.WriteLine($"SQL: {ex.InnerException?.InnerException?.Message}");

                    ModelState.AddModelError("", "An error occurred while saving data to the database.");
                    return View("Index", model);
                }
            }
            return View("Index", model);
        }

        private async Task SetDefaultLicence(LicencePurchaseViewModel viewModel)
        {
            var licence = await _context.Licences.FirstOrDefaultAsync(l => l.LicenceId == 1);

            if (licence != null)
            {
                viewModel.LicenceName = licence.Name;
                viewModel.LicenceCost = licence.Cost;
                viewModel.Quantity = 1;
                viewModel.TotalCost = viewModel.Quantity * licence.Cost;
            }
            else
            {
                viewModel.LicenceName = "Default Licence Name";
                viewModel.LicenceCost = 0;
                viewModel.Quantity = 1;
                viewModel.TotalCost = 0;
            }
        }

        private async Task<int> GetLicenceId(string licenceName)
        {
            var licence = await _context.Licences.FirstOrDefaultAsync(l => l.Name == licenceName);

            if (licence != null)
            {
                return licence.LicenceId;
            }
            else
            {
                return 1;
            }
        }
    }
}
