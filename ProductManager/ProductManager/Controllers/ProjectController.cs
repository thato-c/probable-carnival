using Microsoft.AspNetCore.Mvc;
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
                    Console.WriteLine("Model State is Valid");

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

                    await _context.SaveChangesAsync();
                    return View("Index");
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

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> AssignUser(string checkedUsernames)
        //{
        //    List<string> checkedUsernamesList = checkedUsernames.Split(',').ToList();

        //    if (checkedUsernamesList != null && checkedUsernamesList.Any())
        //    {
        //        var companyId = await _context.Users
        //                .Where(u => u.Username == checkedUsernamesList[0])
        //                .Select(u => u.CompanyId)
        //                .FirstOrDefaultAsync();

        //        for (int i = 0; i < checkedUsernamesList.Count; i++)
        //        {
        //            var userId = await _context.Users
        //                .Where(u => u.Username == checkedUsernamesList[i])
        //                .Select(u => u.UserId)
        //                .FirstOrDefaultAsync();

        //            var Assignment = new UserProjectAssignment
        //            {
        //                UserId = userId,
        //                ProjectId = ViewBag.ProjectId,
        //            };
        //            _context.Add(Assignment);
        //        }

        //        await _context.SaveChangesAsync();
        //        return RedirectToAction("Index", new { companyId });
        //    }
        //    else
        //    {
        //        TempData["UserAssignmentError"] = "No users have been selected.";
        //        return RedirectToAction("UserAssignment");
        //    }
        //}
    }
}
