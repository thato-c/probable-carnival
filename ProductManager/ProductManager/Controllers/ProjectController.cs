using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductManager.Data;
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

        public async Task<IActionResult> Index(int companyId)
        {
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

                return RedirectToAction("Index", new {companyId = model.CompanyId});
            }
            else
            {
                return View("Create", model);
            }
        }
    }
}
