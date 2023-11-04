using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProductManager.Data;
using ProductManager.Models;

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
    }
}
