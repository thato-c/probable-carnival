using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProductManager.Data;
using ProductManager.Models;
using ProductManager.ViewModels;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CompanyDetailsViewModel viewModel)
        {
            try
            {
                // 1. Verify Email
                // 2. Confirm Payment

                if (ModelState.IsValid)
                {
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
                var licences = await _context.Licences
                        .Select(l => new LicenceDropDownItem
                        {
                            Value = l.LicenceId.ToString(),
                            Text = l.Name,
                            Cost = l.Cost.ToString()
                        }).ToListAsync();

                var registeredCompany = await _context.Companies
                    .Where(c => c.CompanyId == CompanyId)
                    .Select(c => new CompanyDetailsViewModel
                    {
                        CompanyId = c.CompanyId,
                        CompanyName = c.CompanyName,
                        CompanyPhoneNumber = c.CompanyPhoneNumber,
                        CompanyEmail = c.CompanyEmail,
                        SelectedLicenceId = c.LicencePurchases
                            .Select(p => p.LicenceId)
                            .FirstOrDefault(),
                        AdminEmail = c.Users
                            .Select(u => u.Username)
                            .FirstOrDefault() ?? "",
                        Licences = licences,
                        Quantity = c.LicencePurchases.Sum(purchase => (int?)purchase.Quantity) ?? 0,
                        TotalCost = c.LicencePurchases.Sum(purchase => (decimal?)purchase.TotalCost) ?? 0,
                        PurchaseDate = c.LicencePurchases
                            .Select(p => (DateTime?)p.PurchaseDate)
                            .FirstOrDefault() ?? DateTime.Now,
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CompanyDetailsViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var previousPurchase = await _context.LicencePurchases
                        .Where(p => p.CompanyId == viewModel.CompanyId)
                        .FirstOrDefaultAsync();             

                    if (previousPurchase == null)
                    {
                        var purchase = new Models.LicencePurchase
                        {
                            CompanyId = viewModel.CompanyId,
                            LicenceId = viewModel.SelectedLicenceId,
                            Quantity = viewModel.Quantity,
                            PurchaseDate = viewModel.PurchaseDate,
                            TotalCost = viewModel.TotalCost
                        };
                        _context.LicencePurchases.Add(purchase);

                        for (var i = 0; i < viewModel.Quantity; i++)
                        {
                            var user = new Models.User
                            {
                                CompanyId = viewModel.CompanyId,
                                Username = await GenerateUsername(viewModel.CompanyName),
                                Password = GenerateUserPassword(),
                            };
                            _context.Users.Add(user);
                        }
                    }
                    else
                    {
                        // Check if the data sent is similar to the data sent to the view initially
                        if (previousPurchase.Quantity == viewModel.Quantity && previousPurchase.LicenceId == viewModel.SelectedLicenceId)
                        {
                            var newLicences = await _context.Licences
                                    .Select(l => new LicenceDropDownItem
                                    {
                                        Value = l.LicenceId.ToString(),
                                        Text = l.Name,
                                        Cost = l.Cost.ToString()
                                    }).ToListAsync();

                            var newModel = new CompanyDetailsViewModel
                            {
                                CompanyId = viewModel.CompanyId,
                                CompanyName = viewModel.CompanyName,
                                CompanyPhoneNumber = viewModel.CompanyPhoneNumber,
                                CompanyEmail = viewModel.CompanyEmail,
                                SelectedLicenceId = viewModel.SelectedLicenceId,
                                AdminEmail = viewModel.AdminEmail,
                                Licences = newLicences,
                                Quantity = previousPurchase.Quantity,
                                TotalCost = previousPurchase.TotalCost,
                                PurchaseDate = previousPurchase.PurchaseDate,
                            };

                            ModelState.AddModelError("", "The data has not been modified");
                            return View(newModel);
                        }
                        else
                        {
                            // In the event that the Quantity has been increased
                            var newUsers = viewModel.Quantity - previousPurchase.Quantity;

                            // These properties will be updated
                            previousPurchase.LicenceId = viewModel.SelectedLicenceId;
                            previousPurchase.Quantity = viewModel.Quantity;
                            previousPurchase.PurchaseDate = DateTime.Now;
                            previousPurchase.TotalCost = viewModel.TotalCost;
                            _context.Entry(previousPurchase).State = EntityState.Modified;

                            if (newUsers > 0)
                            {
                                for (var i = 0; i < newUsers; i++)
                                {
                                    var user = new Models.User
                                    {
                                        CompanyId = viewModel.CompanyId,
                                        Username = await GenerateUsername(viewModel.CompanyName),
                                        Password = GenerateUserPassword(),
                                    };
                                    _context.Users.Add(user);
                                }
                            }
                            else if (newUsers < 0)
                            {
                                var newLicences = await _context.Licences
                                    .Select(l => new LicenceDropDownItem
                                    {
                                        Value = l.LicenceId.ToString(),
                                        Text = l.Name,
                                        Cost = l.Cost.ToString()
                                    }).ToListAsync();

                                var newModel = new CompanyDetailsViewModel
                                {
                                    CompanyId = viewModel.CompanyId,
                                    CompanyName = viewModel.CompanyName,
                                    CompanyPhoneNumber = viewModel.CompanyPhoneNumber,
                                    CompanyEmail = viewModel.CompanyEmail,
                                    SelectedLicenceId = viewModel.SelectedLicenceId,
                                    AdminEmail = viewModel.AdminEmail,
                                    Licences = newLicences,
                                    Quantity = previousPurchase.Quantity,
                                    TotalCost = previousPurchase.TotalCost,
                                    PurchaseDate = previousPurchase.PurchaseDate,
                                };

                                ModelState.AddModelError("", "The quantity of users cannot be decreased from this view");
                                return View(newModel);
                            }
                        }      
                    }
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }

                var licences = await _context.Licences
                    .Select(l => new LicenceDropDownItem
                    {
                        Value = l.LicenceId.ToString(),
                        Text = l.Name,
                        Cost = l.Cost.ToString()
                    }).ToListAsync();

                var model = new CompanyDetailsViewModel
                {
                    CompanyId = viewModel.CompanyId,
                    CompanyName = viewModel.CompanyName,
                    CompanyPhoneNumber = viewModel.CompanyPhoneNumber,
                    CompanyEmail = viewModel.CompanyEmail,
                    SelectedLicenceId = viewModel.SelectedLicenceId,
                    AdminEmail = viewModel.AdminEmail,
                    Licences = licences,
                    Quantity = viewModel.Quantity,
                    PurchaseDate = viewModel.PurchaseDate,
                    TotalCost = viewModel.TotalCost,
                };

                return View(model);
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

                var licences = await _context.Licences
                    .Select(l => new LicenceDropDownItem
                    {
                        Value = l.LicenceId.ToString(),
                        Text = l.Name,
                        Cost = l.Cost.ToString()
                    }).ToListAsync();

                var model = new CompanyDetailsViewModel
                {
                    CompanyId = viewModel.CompanyId,
                    CompanyName = viewModel.CompanyName,
                    CompanyPhoneNumber = viewModel.CompanyPhoneNumber,
                    CompanyEmail = viewModel.CompanyEmail,
                    SelectedLicenceId = viewModel.SelectedLicenceId,
                    AdminEmail = viewModel.AdminEmail,
                    Licences = licences,
                    Quantity = viewModel.Quantity,
                    PurchaseDate = viewModel.PurchaseDate,
                    TotalCost = viewModel.TotalCost,
                };
                return View(model);
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
