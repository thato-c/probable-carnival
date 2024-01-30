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
    public class LoginController : Controller
    {
        public readonly ApplicationDBContext _context;

        public LoginController(ApplicationDBContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _context.Users
                        .Include(u => u.UserProjectAssignments)
                        .Include(u => u.UserRoles)
                            .ThenInclude(ur => ur.Role)
                        .FirstOrDefaultAsync(u => u.Username == model.Username);

                    if (user != null && BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
                    {
                        // Create a list to hold the claims
                        var claims = new List<Claim>();

                        // Add a ClaimType for username
                        var username = user.Username;
                        claims.Add(new Claim(ClaimTypes.Name, username.ToString()));

                        var userRole = await _context.UserRoles.FirstOrDefaultAsync(u => u.UserId == user.UserId);

                        if (user.UserProjectAssignments != null && user.UserProjectAssignments.Any())
                        {
                            // User has projects assigned, redirect to a page related to their project.
                            var projectName = await (
                                from project in _context.Projects
                                join assignment in _context.UserProjectsAssignments on project.ProjectId equals assignment.ProjectId
                                where assignment.UserId == user.UserId
                                select project.Name
                            ).FirstOrDefaultAsync();

                            if (userRole != null && userRole.Role.Name != null)
                            {
                                claims.Add(new Claim(ClaimTypes.Role, userRole.Role.Name));
                                claims.Add(new Claim("ProjectName", projectName.ToString()));

                                // Create a ClaimsIdentity and attach the claims to it.
                                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                                // Create a ClaimasPrincipal with the ClaaimsIdentity
                                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                                // Sign in the user with the ClaimsPrincipal
                                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
                            };

                            var projectId = user.UserProjectAssignments.First().ProjectId;

                            //return RedirectToAction("ProjectDetails", "Projects", new { projectId });
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {

                            // User has no projects assigned

                            // User has a role of User
                            if (userRole != null && userRole.Role != null && userRole.Role.Name == "User")
                            {
                                claims.Add(new Claim(ClaimTypes.Role, userRole.Role.Name));

                                // Create a ClaimsIdentity and attach the claims to it.
                                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                                // Create a ClaimsPrincipal with the ClaimsIdentity
                                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                                // Sign in the user with the ClaimsPrincipal
                                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

                                return View("ContactAdmin");
                            }

                            // User  has a role of Company Admin
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
                            Payment = "Unpaid",
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
