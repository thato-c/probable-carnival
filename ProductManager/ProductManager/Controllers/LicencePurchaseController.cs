using Microsoft.AspNetCore.Mvc;
using NuGet.Packaging.Licenses;
using ProductManager.Data;
using ProductManager.ViewModels;

namespace ProductManager.Controllers
{
    public class LicencePurchaseController : Controller
    {

        private readonly ApplicationDBContext _context;

        public LicencePurchaseController(ApplicationDBContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var viewModel = new LicencePurchaseViewModel();
            return View(viewModel);
        }
    }
}
