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

            var users = _context.Users
                .Where(u => u.CompanyId == companyIdentity )
                .ToList();

            var userViewModel = new UserViewModel
            {
                Users = new List<UserRegistrationViewModel>(),
            };

            foreach (var user in users)
            {
                userViewModel.Users.Add(new UserRegistrationViewModel 
                {
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
    }
}
