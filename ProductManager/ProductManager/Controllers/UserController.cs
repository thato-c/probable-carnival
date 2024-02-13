using Microsoft.AspNetCore.Mvc;
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

        public IActionResult Index()
        {
            var companyIdentity = getCompanyId();

            var companyName = _context.Companies.Where(c => c.CompanyId == companyIdentity).FirstOrDefault();

            var users = _context.Users
                .Where(u => u.CompanyId == companyIdentity )
                .ToList();

            var userViewModel = new UserViewModel
            {
                CompanyName = companyName.ToString(),
                Users = new List<UserRegistrationViewModel>(),
            };

            foreach (var user in users)
            {
                userViewModel.Users.Add(new UserRegistrationViewModel 
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    Password = user.Password,
                });
            }

            return View(userViewModel);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(UserRegistrationViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users.Where(u => u.UserId == viewModel.UserId).FirstOrDefault();

                if (user != null)
                {
                    user.Username = viewModel.Username;
                    user.Password = viewModel.Password;
                    _context.Entry(user).State = EntityState.Modified;
                    _context.SaveChanges();
                }
            }

            var companyIdentity = getCompanyId();

            var companyName = _context.Companies.Where(c => c.CompanyId == companyIdentity).FirstOrDefault();

            var users = _context.Users
                .Where(u => u.CompanyId == companyIdentity)
                .ToList();

            var userViewModel = new UserViewModel
            {
                CompanyName = companyName.ToString(),
                Users = new List<UserRegistrationViewModel>(),
            };

            foreach (var user in users)
            {
                userViewModel.Users.Add(new UserRegistrationViewModel
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    Password = user.Password,
                });
            }

            return View("Index", userViewModel);
        }
    }
}
