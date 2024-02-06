using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using ProductManager.Data;
using ProductManager.Models;
using ProductManager.ViewModels;

namespace ProductManager.Controllers
{
    public class DocumentController : Controller
    {
        public readonly ApplicationDBContext _context;

        public DocumentController(ApplicationDBContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Get project Name
            string currentUserProjectName = GetCurrentUserProjectName();

            try
            {
                // Get projectId
                var projectId = _context.Projects
                    .Where(p => p.Name == currentUserProjectName)
                    .Select(p => p.ProjectId)
                    .FirstOrDefault();

                // Get project documents
                var filteredDocuments = _context.Documents
                    .Where(d => d.ProjectId == projectId)
                    .ToList();

                var documentUploadViewModel = new DocumentUploadViewModel
                {
                    Documents = new List<DocumentViewModel>(),
                    FileUpload = new BufferedSingleFileUploadDb(),
                    ProjectId = projectId,
                };

                for (int i = 0; i < filteredDocuments.Count; i++)
                {
                    documentUploadViewModel.Documents.Add(new DocumentViewModel());
                }

                return View(documentUploadViewModel);
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

        public async Task<IActionResult> UploadFile(DocumentUploadViewModel viewModel)
        {
            using (var memoryStream = new MemoryStream())
            {
                await viewModel.FileUpload.FormFile.CopyToAsync(memoryStream);

                // Upload the file if less thsn 2MB
                if (memoryStream.Length < 2097152)
                {
                    var file = new Models.Document()
                    {
                        Name = "Document1",
                        FileSize = memoryStream.Length,
                        UploadDate = DateTime.Now,
                        FileURL = "Database",
                        ProjectId = viewModel.ProjectId,
                        //Content = memoryStream.ToArray()
                    };

                    _context.Documents.Add(file);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    ModelState.AddModelError("File", "The file is too large");
                }
            }

            // Get project documents
            var filteredDocuments = _context.Documents
                .Where(d => d.ProjectId == viewModel.ProjectId)
                .ToList();

            var documentUploadViewModel = new DocumentUploadViewModel
            {
                Documents = new List<DocumentViewModel>(),
                FileUpload = new BufferedSingleFileUploadDb(),
                ProjectId = viewModel.ProjectId,
            };

            for (int i = 0; i < filteredDocuments.Count; i++)
            {
                documentUploadViewModel.Documents.Add(new DocumentViewModel());
            }

            return View("Index", documentUploadViewModel);
        }


    }
}
