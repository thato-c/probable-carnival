using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductManager.Data;
using ProductManager.ViewModels;

namespace ProductManager.Controllers
{
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
                    if (user.UserProjectAssignments != null && user.UserProjectAssignments.Any())
                    {
                        // User has projects assigned, redirect to a page related to their project.
                        var projectId = user.UserProjectAssignments.First().ProjectId;
                        //return RedirectToAction("ProjectDetails", "Projects", new { projectId });
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        // User has no projects assigned
                        var userRole = await _context.UserRoles.FirstOrDefaultAsync(u => u.UserId == user.UserId);

                        // User has a role of User
                        if (userRole != null && userRole.Role != null && userRole.Role.Name == "User")
                        {
                            return View("ContactAdmin");
                        }
                        // User  has a role of Company Admin
                        else if (userRole != null && userRole.Role != null && userRole.Role.Name == "Company Administrator")
                        {
                            var companyHasProjects = _context.Projects.Any(p => p.CompanyId == user.CompanyId);

                            if (companyHasProjects)
                            {
                                return RedirectToAction("Index", "Home");
                            }
                            else
                            {
                                // Redirect to Create a project
                                return RedirectToAction("Create", "Project");
                            }
                        }
                        else if(userRole == null)
                        {
                            ModelState.AddModelError("", "UserRole is null");
                            return View("Index", model);
                        }
                        else if (userRole.Role == null)
                        {
                            ModelState.AddModelError("", "UserRole is null");
                            return View("Index", model);
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
