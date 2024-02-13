using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductManager.Data;
using ProductManager.Models;
using ProductManager.ViewModels;
using System.Security.Claims;

namespace ProductManager.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        public readonly ApplicationDBContext _context;

        public AccountController(ApplicationDBContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        //

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _context.Users
                        .Include(u => u.Company)
                        .FirstOrDefaultAsync(u => u.Username == model.Username);

                    if (user != null && BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
                    {
                        // Create a list to hold the claims
                        var claims = new List<Claim>();

                        // Add a ClaimType for username
                        var username = user.Username;
                        var companyId = user.Company.CompanyId;
                        claims.Add(new Claim(ClaimTypes.Name, username.ToString()));
                        claims.Add(new Claim("CompanyId", companyId.ToString()));

                        var userRole = await _context.UserRoles
                            .Include(ur => ur.Role)
                            .FirstOrDefaultAsync(u => u.UserId == user.UserId);

                        // When the user is a Company Admin
                        if (userRole != null && userRole.Role != null && userRole.Role.Name == "Vendor")
                        {
                            claims.Add(new Claim(ClaimTypes.Role, userRole.Role.Name));

                            // Create a ClaimsIdentity and attach the claims to it.
                            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                            // Create a ClaimsPrincipal with the ClaimsIdentity
                            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                            // Sign in the user with the ClaimsPrincipal
                            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
                            return RedirectToAction("Index", "Company");
                        }
                        else if (userRole != null && userRole.Role != null && userRole.Role.Name == "Company Administrator")
                        {
                            claims.Add(new Claim(ClaimTypes.Role, userRole.Role.Name));

                            // Create a ClaimsIdentity and attach the claims to it.
                            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                            // Create a ClaimsPrincipal with the ClaimsIdentity
                            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                            // Sign in the user with the ClaimsPrincipal
                            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
                            return RedirectToAction("Index", "Project", new { companyId = user.CompanyId });
                        }
                        else if (userRole != null && userRole.Role != null && userRole.Role.Name == "User")
                        {
                            var userProject = await _context.UserProjectsAssignments
                                .Include(u => u.Project)
                                .FirstOrDefaultAsync(u => u.UserId == user.UserId);

                            if (userProject != null)
                            {
                                if (userRole != null && userRole.Role.Name != null)
                                {
                                    claims.Add(new Claim(ClaimTypes.Role, userRole.Role.Name));
                                    claims.Add(new Claim("ProjectName", userProject.Project.Name.ToString()));

                                    // Create a ClaimsIdentity and attach the claims to it.
                                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                                    // Create a ClaimasPrincipal with the ClaaimsIdentity
                                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                                    // Sign in the user with the ClaimsPrincipal
                                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
                                };

                                return RedirectToAction("Index", "Home");
                            }
                            else
                            {

                                // User has no projects assigned
                                claims.Add(new Claim(ClaimTypes.Role, userRole.Role.Name));

                                // Create a ClaimsIdentity and attach the claims to it.
                                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                                // Create a ClaimsPrincipal with the ClaimsIdentity
                                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                                // Sign in the user with the ClaimsPrincipal
                                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

                                return View("ContactAdmin");

                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Invalid Credentials");
                        return View("Index", model);
                    }
                }
                return View("Index", model);
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
                return View("Index", model);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegistrationViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Check if the Company already exists
                    var existingCompany = await _context.Companies.FirstOrDefaultAsync(c =>
                        c.CompanyName == model.CompanyName ||
                        c.CompanyEmail == model.CompanyEmail ||
                        c.AdminEmail == model.AdminEmail ||
                        c.CompanyPhoneNumber == model.CompanyPhoneNumber);

                    if (existingCompany != null)
                    {
                        ModelState.AddModelError("", "Company already exists");
                        return View(model);
                    }
                    else
                    {
                        // Map the viewModel to the Company entity
                        var Company = new Models.Company
                        {
                            CompanyName = model.CompanyName,
                            CompanyEmail = model.CompanyEmail,
                            CompanyPhoneNumber = model.CompanyPhoneNumber,
                            AdminEmail = model.AdminEmail,
                            PaymentStatus = PaymentStatus.Unpaid,
                        };
                        _context.Companies.Add(Company);
                        _context.SaveChanges();

                        return RedirectToAction("RegistrationSuccess");
                    }
                }
                else
                {
                    return View(model);
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
                return View(model);
            }

        }

        [HttpGet]
        public IActionResult RegistrationSuccess()
        {
            return View();
        }

        private string GenerateUserPassword()
        {
            return "qwe123!Q";
        }

        public IActionResult ContactAdmin()
        {
            return View();
        }
    }
}
