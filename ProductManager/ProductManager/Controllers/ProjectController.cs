using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductManager.Data;

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

        public IActionResult Create()
        {
            return View();
        }
    }
}
