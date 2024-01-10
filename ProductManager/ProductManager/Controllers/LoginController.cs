using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductManager.Data;
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
        public async Task<IActionResult> Login(LoginViewModel model)
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

                    var userRole = await _context.UserRoles.FirstOrDefaultAsync(u => u.UserId == user.UserId);

                    if (user.UserProjectAssignments != null && user.UserProjectAssignments.Any())
                    {
                        // User has projects assigned, redirect to a page related to their project.
                        var projectId = user.UserProjectAssignments.First().ProjectId;

                        if (userRole != null && userRole.Role.Name != null)
                        {
                            claims.Add(new Claim(ClaimTypes.Role, userRole.Role.Name));
                            claims.Add(new Claim("ProjectId", projectId.ToString()));
                            
                            // Create a ClaimsIdentity and attach the claims to it.
                            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                            // Create a ClaimasPrincipal with the ClaaimsIdentity
                            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                            // Sign in the user with the ClaimsPrincipal
                            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
                        };

                        //return RedirectToAction("ProjectDetails", "Projects", new { projectId });
                        return RedirectToAction("Index", "Home", new {ProjectId = projectId});
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
                            var companyHasProjects = _context.Projects.Any(p => p.CompanyId == user.CompanyId);

                            if (companyHasProjects)
                            {
                                claims.Add(new Claim(ClaimTypes.Role, userRole.Role.Name));

                                // Create a ClaimsIdentity and attach the claims to it.
                                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                                // Create a ClaimsPrincipal with the ClaimsIdentity
                                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                                // Sign in the user with the ClaimsPrincipal
                                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
                                return RedirectToAction("Index", "Project", new {companyId = user.CompanyId});
                            }
                            else
                            {
                                claims.Add(new Claim(ClaimTypes.Role, userRole.Role.Name));

                                // Create a ClaimsIdentity and attach the claims to it.
                                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                                // Create a ClaimsPrincipal with the ClaimsIdentity
                                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                                // Sign in the user with the ClaimsPrincipal
                                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
                                return RedirectToAction("Create", "Project", new {companyId = user.CompanyId});
                            }
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

        public IActionResult ContactAdmin()
        {
            return View();
        }
    }
}
