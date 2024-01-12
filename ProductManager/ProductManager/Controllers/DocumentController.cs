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

            // Get projectId
            var projectId = await _context.Projects
                .Where(p => p.Name == currentUserProjectName)
                .Select(p => p.ProjectId)
                .FirstOrDefaultAsync();

            // Get project documents
            var filteredDocuments = _context.Documents
                .Where(d => d.ProjectId == projectId)
                .ToListAsync();

            //var filteredDocuments = await (
            //    from project in _context.Projects
            //    join document in _context.Documents on project.ProjectId equals document.ProjectId
            //    where document.Name == currentUserProjectName
            //    select document.Name
            //).ToListAsync();

            return View(await filteredDocuments);
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
