using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductManager.Data;
using ProductManager.ViewModels;

namespace ProductManager.Controllers
{
    public class RoleController : Controller
    {
        private readonly ApplicationDBContext _context;

        public RoleController(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Retrieve a list of roles from the database
            var roles = await _context.Roles.ToListAsync();

            if (roles.Count == 0)
            {
                ViewBag.Message = "No Roles have been registered";
                return View();
            }

            return View(roles);
        }

        [HttpGet] 
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Map the ViewModel to the Role entity
                var role = new Models.Role
                {
                    Name = model.Name,
                    Description = model.Description,
                };

                // Add and save the new role to the database
                _context.Roles.Add(role);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return View(model);
        }
    }
}
