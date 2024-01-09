using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProductManager.Data;
using ProductManager.Models;
using ProductManager.ViewModels;

namespace ProductManager.Controllers
{
    public class ProjectController : Controller
    {
        public readonly ApplicationDBContext _context;

        public ProjectController(ApplicationDBContext context) 
        { 
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int companyId)
        {
            ViewBag.CompanyId = companyId;
            var projects = await _context.Projects.Where(p => p.CompanyId == companyId).ToListAsync();

            if (projects != null && projects.Any())
            {
                return View(projects);
            }
            else
            {
                ViewBag.Message = "No Projects have been created.";
                return View();
            }   
        }

        [HttpGet]
        public IActionResult Create(int companyId)
        {
            var ProjectViewModel = new ProjectViewModel
            {
                CompanyId = companyId,
            };

            return View(ProjectViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProject(ProjectViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Map the ViewModel to Project Entity
                var project = new Models.Project
                {
                    Name = model.Name,
                    CompanyId = model.CompanyId,
                };

                // Add and save the new project to the database
                _context.Projects.Add(project);
                await _context.SaveChangesAsync();

                // Send the CompanyId and ProjectId to the UserProjectAssignmentViewModel
                var UserAssignmentViewModel = new UserAssignmentViewModel
                {
                    CompanyId = model.CompanyId,
                    ProjectId = project.ProjectId,
                };

                return RedirectToAction("UserAssignment", UserAssignmentViewModel);
            }
            else
            {
                return View("Create", model);
            }
        }

        // Display the list of users (within the company)
        [HttpGet]
        public async Task<IActionResult> UserAssignment(UserAssignmentViewModel viewModel)
        {
            var users = await _context.Users
                .Where(u => u.CompanyId == viewModel.CompanyId)
                .Select(u => new ProjectUserViewModel
                {
                    Username = u.Username,
                    IsSelected = false
                })
                .ToListAsync();

            if (users != null && users.Any())
            {
                // Call ProjectUserAssignmentViewModel with the list of retrieved users
                var ProjectUserAssignmentViewModel = new ProjectUserAssignmentViewModel
                {
                    ProjectId = viewModel.ProjectId,
                    CompanyId = viewModel.CompanyId,
                    ProjectUsers = users,
                };

                return View(ProjectUserAssignmentViewModel);
            }
            else
            {
                // No users Have been assigned to the company
                ViewBag.ProjectId = viewModel.ProjectId;
                return View();
            }
        }

        // Assign selected users to the project
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserAssignmentPost(ProjectUserAssignmentViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var selectedUsernames = viewModel.ProjectUsers
                        .Where(u => u.IsSelected)
                        .Select(u => u.Username)
                        .ToList();

                    foreach (var user in selectedUsernames)
                    {
                        var userId = await _context.Users
                            .Where(u => u.Username == user)
                            .Select(u => u.UserId)
                            .FirstOrDefaultAsync();

                        // Map viewModel to UserProjectAssignment entity
                        var Assignment = new Models.UserProjectAssignment
                        {
                            ProjectId = viewModel.ProjectId,
                            UserId = userId
                        };

                        _context.UserProjectsAssignments.Add(Assignment);
                    }

                    int CompanyId = viewModel.CompanyId;

                    await _context.SaveChangesAsync();

                    ViewBag.Roles = new SelectList(new List<SelectListItem>
                    {
                        new SelectListItem { Value = "Project Admin", Text = "Project Admin"},
                        new SelectListItem { Value = "Read and Write", Text = "Read and Write"},
                        new SelectListItem { Value = "Read Only", Text = "Read Only"}
                    }, "Value", "Text");

                    var projectUserRoles = await (
                        from userProjectAssignment in _context.UserProjectsAssignments
                        join project in _context.Users on userProjectAssignment.UserId equals project.UserId
                        where userProjectAssignment.ProjectId == viewModel.ProjectId
                        select new UserAssignmentRoleViewModel
                        {
                            Username = project.Username,
                            SelectedRole = "Read Only", // Set to Read
                            AssignmentId = userProjectAssignment.AssignmentId,
                        }).ToListAsync();

                    var projectUserAssignmentRoleViewModel = new ProjectUserAssignmentRoleViewModel
                    {
                        ProjectId = viewModel.ProjectId,
                        CompanyId = viewModel.CompanyId,
                        ProjectUserRoles = projectUserRoles,
                    };

                    return View("ProjectRole", projectUserAssignmentRoleViewModel);
                }
                else
                {
                    return View("UserAssignment", viewModel);
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
                return View("UserAssignment", viewModel);
            }
        }

        [HttpGet]
        public async Task<IActionResult> ProjectRole(ProjectUserAssignmentRoleViewModel viewModel)
        {
            try
            {
                ViewBag.Roles = new SelectList(new List<SelectListItem>
                {
                    new SelectListItem { Value = "Project Admin", Text = "Project Admin"},
                    new SelectListItem { Value = "Read and Write", Text = "Read and Write"},
                    new SelectListItem { Value = "Read Only", Text = "Read Only"}
                }, "Value", "Text");

                var projectUserRoles = await (
                    from userProjectAssignment in _context.UserProjectsAssignments
                    join assignment in _context.UserProjectRoles on userProjectAssignment.AssignmentId equals assignment.AssignmentId
                    join role in _context.Roles on assignment.RoleId equals role.RoleId
                    join user in _context.Users on assignment.UserProjectAssignment.UserId equals user.UserId
                    where userProjectAssignment.ProjectId == viewModel.ProjectId
                    select new UserAssignmentRoleViewModel
                    {
                        SelectedRole = role.Name,
                        Username = user.Username,
                        AssignmentId = assignment.AssignmentId,
                    }).ToListAsync();

                // There are roles associated with ProjectUsers already
                if (projectUserRoles != null &&  projectUserRoles.Any() )
                {
                    var projectUserAssignmentRoleViewModel = new ProjectUserAssignmentRoleViewModel
                    {
                        ProjectId = viewModel.ProjectId,
                        CompanyId = viewModel.CompanyId,
                        ProjectUserRoles = projectUserRoles,
                    };

                    return View(projectUserRoles.ToArray());
                }
                else
                {
                    return View();
                }
            } catch (DbUpdateException ex)
            {
                // Log the exception details
                Console.WriteLine($"DbUpdateException: {ex.Message}");
                Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");

                // Optionally, log additional details
                // Log the SQL statement causing the exception
                Console.WriteLine($"SQL: {ex.InnerException?.InnerException?.Message}");

                ModelState.AddModelError("", "An error occurred while saving data to the database.");
                return View("UserAssignment", viewModel);
            }
        }
    }
}
