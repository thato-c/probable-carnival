using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using ProductManager.Data;
using ProductManager.Models;
using ProductManager.ViewModels;

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

        [HttpPost]
        [ValidateAntiForgeryToken]
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

                    // Map the Quantity to the User entity
                    var UserViewModel = new UserViewModel
                    {
                        CompanyId = Company.CompanyId,
                        Quantity = model.Quantity,
                        Users = new List<UserRegistrationViewModel>()
                    };

                    for (int i = 0; i < model.Quantity; i++)
                    {
                        UserViewModel.Users.Add(new UserRegistrationViewModel());
                    }

                    return View(UserViewModel);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreatePost(UserViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                for (int i = 0; i < viewModel.Quantity; i++)
                {
                    var hashedPassword = BCrypt.Net.BCrypt.HashPassword(viewModel.Users[i].Password);

                    var user = new User
                    {
                        CompanyId = viewModel.CompanyId,
                        Username = viewModel.Users[i].Username,
                        Password = hashedPassword
                    };

                    // Add the user to the database
                    _context.Users.Add(user);
                }

                // Save the changes to the database
                _context.SaveChanges();
                return RedirectToAction("Index", "Home");
            } 
            else
            {
                return View("Create", viewModel);
            }
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
