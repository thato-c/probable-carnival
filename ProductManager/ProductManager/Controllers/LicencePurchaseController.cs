using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging.Licenses;
using ProductManager.Data;
using ProductManager.Models;
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

        public async Task<IActionResult> Index()
        {
            var viewModel = new LicencePurchaseViewModel();

            var licence = await _context.Licences.FirstOrDefaultAsync(l => l.LicenceId == 1);

            if (licence != null)
            {
                viewModel.LicenceDetails = new LicenceViewModel();
                viewModel.LicenceDetails.Name = licence.Name;
            }
            else
            {
                viewModel.LicenceDetails = new LicenceViewModel();
                viewModel.LicenceDetails.Name = "Default Licence Name";
            } 

            return View(viewModel);
        }
    }
}
