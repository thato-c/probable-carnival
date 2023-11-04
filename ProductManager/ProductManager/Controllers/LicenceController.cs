using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProductManager.Data;
using ProductManager.Models;
using ProductManager.ViewModels;

namespace ProductManager.Controllers
{
    public class LicenceController : Controller
    {
        private readonly ApplicationDBContext _context;

        public LicenceController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: Licences
        public async Task<IActionResult> Index()
        {
            // Retrieve a list of licences from the database
            var licences = await _context.Licences.ToListAsync();

            if (licences.Count == 0)
            {
                ViewBag.Message = "No Licences Registered";
                return View();
            }

            return View(licences);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LicenceViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Map the ViewModel to Licence entity
                var Licence = new Models.Licence
                {
                    Name = model.Name,
                    Description = model.Description,
                    Cost = model.Cost,
                    ValidityMonths = model.ValidityMonths
                };

                // Add and save the new licence to the database
                _context.Licences.Add(Licence);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return View(model);
        }
    }
}
