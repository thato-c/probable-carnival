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

        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u =>
                        u.Username == model.Username);

                if (user != null && BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            return View("Index", model);
        }
    }
}
