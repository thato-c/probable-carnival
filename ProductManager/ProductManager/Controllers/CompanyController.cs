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
            try
            {
                var licences = await _context.Licences
                .Select(l => new LicenceDropDownItem
                {
                    Value = l.LicenceId.ToString(),
                    Text = l.Name,
                    Cost = l.Cost.ToString()
                }).ToListAsync();

                var viewModel = new CompanyDetailsViewModel
                {
                    Licences = licences,
                    Quantity = 1,
                    PurchaseDate = DateTime.Now,
                };

                return View(viewModel);
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

        [HttpPost]
        public async Task<IActionResult> Create(CompanyDetailsViewModel viewModel)
        {
            try
            {
                // 1. Verify Email
                // 2. Confirm Payment

                if (ModelState.IsValid)
                {
                    // Check if the Company already exists

                    var existingCompany = await _context.Companies.FirstOrDefaultAsync(c =>
                        c.CompanyName == viewModel.CompanyName ||
                        c.CompanyEmail == viewModel.CompanyEmail ||
                        c.CompanyPhoneNumber == viewModel.CompanyPhoneNumber);

                    var existingAdmin = await _context.Users.FirstOrDefaultAsync(u =>
                        u.Username == viewModel.AdminEmail
                    );

                    if (existingCompany != null || existingAdmin != null)
                    {
                        ModelState.AddModelError("", "Company with the same details already exists.");
                        var licences = await _context.Licences
                        .Select(l => new LicenceDropDownItem
                        {
                            Value = l.LicenceId.ToString(),
                            Text = l.Name,
                            Cost = l.Cost.ToString()
                        }).ToListAsync();

                        var model = new CompanyDetailsViewModel
                        {
                            Licences = licences,
                            Quantity = 1,
                            PurchaseDate = DateTime.Now,
                        };
                        return View(model);
                    }
                    else
                    {
                        var company = new Models.Company
                        {
                            CompanyName = viewModel.CompanyName,
                            CompanyEmail = viewModel.CompanyEmail,
                            CompanyPhoneNumber = viewModel.CompanyPhoneNumber,
                        };
                        _context.Companies.Add(company);
                        await _context.SaveChangesAsync();

                        var purchase = new Models.LicencePurchase
                        {
                            CompanyId = company.CompanyId,
                            LicenceId = viewModel.SelectedLicenceId,
                            Quantity = viewModel.Quantity,
                            PurchaseDate = viewModel.PurchaseDate,
                            TotalCost = viewModel.TotalCost
                        };
                        _context.LicencePurchases.Add(purchase);

                        var admin = new Models.User
                        {
                            CompanyId = company.CompanyId,
                            Username = viewModel.AdminEmail,
                            Password = GenerateUserPassword(),
                        };
                        _context.Users.Add(admin);

                        for (var i = 0; i < viewModel.Quantity - 1; i++)
                        {
                            var user = new Models.User
                            {
                                CompanyId = company.CompanyId,
                                Username = await GenerateUsername(company.CompanyName),
                                Password = GenerateUserPassword(),
                            };
                            _context.Users.Add(user);
                        }
                        await _context.SaveChangesAsync();

                        return RedirectToAction("Index");
                    }   
                }
                else
                {
                    var licences = await _context.Licences
                    .Select(l => new LicenceDropDownItem
                    {
                        Value = l.LicenceId.ToString(),
                        Text = l.Name,
                        Cost = l.Cost.ToString()
                    }).ToListAsync();

                    var model = new CompanyDetailsViewModel
                    {
                        Licences = licences,
                        Quantity = 1,
                        PurchaseDate = DateTime.Now,
                    };

                    return View("Create", model);
                }
               
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
                return View();
            }
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
                    //SelectedLicenceId = c.LicencePurchases
                    //    .Join(_context.Licences, p => p.LicenceId, l => l.LicenceId, (p, l) => l.Name)
                    //    .FirstOrDefault() ?? "Default Licence Name",
                    AdminEmail = c.Users
                        .Select(u => u.Username)
                        .FirstOrDefault() ?? "Defult Admin Email",
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

        private string GenerateUserPassword()
        {
            var password = "qwe123!Q";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            return hashedPassword;
        }

        private async Task<string> GenerateUsername(string CompanyName)
        {
            string Prefix = "user";
            string Suffix = "@gmail.com";

            while (true)
            {
                Random random = new Random();
                int randomNumber = random.Next(1000, 10000);

                string username = $"{CompanyName}{Prefix}{randomNumber}{Suffix}";

                var userExists = await _context.Users
                    .Where(u => u.Username == username)
                    .Select(u => u.Username)
                    .FirstOrDefaultAsync();

                if (userExists == null)
                {
                    return username;
                }
            }
        }
    }
}
