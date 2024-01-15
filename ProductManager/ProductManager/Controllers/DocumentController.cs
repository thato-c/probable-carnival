using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductManager.Data;

namespace ProductManager.Controllers
{
    public class DocumentController : Controller
    {
        public readonly ApplicationDBContext _context;

        public DocumentController(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Get project Name
            string currentUserProjectName = GetCurrentUserProjectName();

            try
            {
                // Get projectId
                var projectId = await _context.Projects
                    .Where(p => p.Name == currentUserProjectName)
                    .Select(p => p.ProjectId)
                    .FirstOrDefaultAsync();

                // Get project documents
                var filteredDocuments = _context.Documents
                    .Where(d => d.ProjectId == projectId)
                    .ToListAsync();

                return View(await filteredDocuments);
            }
            catch (DbUpdateException ex)
            {
                // Log the exception details
                Console.WriteLine($"DbUpdateException: {ex.Message}");
                Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");

                // Optionally, log additional details
                // Log the SQL statement causing the exception
                Console.WriteLine($"SQL: {ex.InnerException?.InnerException?.Message}");
                ModelState.AddModelError("", "An error occurred while retrieving data from the database.");
                return View();
            }
            
        }

        public string GetCurrentUserProjectName()
        {
            // Check if the user is authenticated
            if (User.Identity.IsAuthenticated)
            {
                // Access the user's claims to find the project name
                var projectClaim = User.FindFirst("ProjectName");

                if (projectClaim != null)
                {
                    return projectClaim.Value;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
    }
}
