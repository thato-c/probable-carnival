﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductManager.Data;
using ProductManager.ViewModels;

namespace ProductManager.Controllers
{
    public class UserController : Controller
    {
        public readonly ApplicationDBContext _context;

        public UserController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var companyIdentity = getCompanyId();

            var companyName = _context.Companies.Where(c => c.CompanyId == companyIdentity).FirstOrDefault();

            var users = _context.Users
                .Where(u => u.CompanyId == companyIdentity)
                .ToList();

            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int UserId)
        {
            var user = _context.Users.Where(u => u.UserId == UserId).FirstOrDefault();

            return View(user);
        }

        public int getCompanyId()
        {
            if (User.Identity.IsAuthenticated)
            {
                var companyClaim = User.FindFirst("CompanyId");

                if (companyClaim != null)
                {
                    int companyId = int.Parse(companyClaim.Value);

                    // Retrieve the companyId
                    return companyId;
                }
                else
                {
                    // User does not have a companyClaim
                    return 0;
                }
            }
            else
            {
                // The user is not authenticated
                return 0;
            }
        }
    }
}
