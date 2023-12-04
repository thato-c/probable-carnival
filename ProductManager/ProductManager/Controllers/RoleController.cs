using Microsoft.AspNetCore.Mvc;

namespace ProductManager.Controllers
{
    public class RoleController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
