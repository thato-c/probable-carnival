using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductManager.Data;
using ProductManager.Models;
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

            var userRoles = new Dictionary<int, string>();

            foreach(var user in users)
            {
                var roleName = _context.UserRoles
                    .Include(r => r.Role)
                    .FirstOrDefault(r => r.UserId == user.UserId)?.Role?.Name;

                userRoles[user.UserId] = roleName ?? "User";
            }

            ViewBag.UserRoles = userRoles;

            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int UserId)
        {
            var user = _context.Users
                .Include(ur => ur.UserRoles)
                    .ThenInclude(r => r.Role)
                .FirstOrDefault(u => u.UserId == UserId);

            var roleName = user.UserRoles.FirstOrDefault()?.Role?.RoleId;

            var includedRoles = new List<string> {"Company Administrator", "User"};

            var roles = await _context.Roles
                .Where(r => includedRoles.Contains(r.Name))
                .Select(r => new LicenceDropDownItem
                {
                    Value = r.RoleId.ToString(),
                    Text = r.Name.ToString(),
                }).ToListAsync();

            var User = new UserRegistrationViewModel 
            { 
                UserId = user.UserId,
                Username = user.Username,
                Password = user.Password,
                SelectedRole = roleName ?? 5,
                Roles = roles
            };

            return View(User);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserRegistrationViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users.Where(u => u.UserId == viewModel.UserId).FirstOrDefault();
                var userRole = _context.UserRoles.Where(ur => ur.UserId == viewModel.UserId).FirstOrDefault();

                if (user != null && userRole != null)
                {
                    user.Username = viewModel.Username;
                    user.Password = viewModel.Password;
                    userRole.RoleId = viewModel.SelectedRole;
                    try
                    {
                        _context.Entry(user).State = EntityState.Modified;
                        _context.Entry(userRole).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "An error occurred while saving changes: " + ex.Message);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "User or user role was not found.");
                }
            }

            var newUser = _context.Users
                .Include(ur => ur.UserRoles)
            .ThenInclude(r => r.Role)
                .FirstOrDefault(u => u.UserId == viewModel.UserId);

            var roleName = newUser.UserRoles.FirstOrDefault()?.Role?.RoleId;

            var includedRoles = new List<string> { "Company Administrator", "User" };

            var roles = await _context.Roles
                .Where(r => includedRoles.Contains(r.Name))
                .Select(r => new LicenceDropDownItem
                {
                    Value = r.RoleId.ToString(),
                    Text = r.Name.ToString(),
                }).ToListAsync();

            var NewUser = new UserRegistrationViewModel
            {
                UserId = newUser.UserId,
                Username = newUser.Username,
                Password = newUser.Password,
                SelectedRole = roleName ?? 5,
                Roles = roles
            };
            return View(NewUser);
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
