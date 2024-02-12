using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Evaluation;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using ProductManager.Data;
using ProductManager.Models;
using ProductManager.ViewModels;
using System.Linq.Expressions;

namespace ProductManager.Controllers
{
    public class DocumentController : Controller
    {
        public readonly ApplicationDBContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public DocumentController(ApplicationDBContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        [HttpGet]
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
                    .Select(d => new DocumentViewModel
                    {
                        FileId = d.DocumentId,
                        Name = d.Name,
                        FileSize = d.FileSize,
                        UploadDate = d.UploadDate,
                    })
                    .ToList();

                var documentUploadViewModel = new DocumentUploadViewModel
                {
                    Documents = filteredDocuments,
                    FileUpload = new BufferedSingleFileUploadDb(),
                    ProjectId = projectId,
                };

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
                    // User does not have a projectClaim and should not be authorized
                    return null;
                }
            }
            else
            {
                // User is not authenticated
                return null;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadFile(DocumentUploadViewModel viewModel)
        {
            // Ensure the file is a pdf
            // Scan the file
            // Change the name


            if (ModelState.IsValid)
            {

                // Ensure the file is a PDF
                if (viewModel.FileUpload.FormFile.ContentType != "application/pdf")
                {
                    ModelState.AddModelError("", "Only PDF files are allowed.");
                    return View("Index", viewModel);
                }

                using (var memoryStream = new MemoryStream())
                {
                    await viewModel.FileUpload.FormFile.CopyToAsync(memoryStream);

                    // Upload the file if less than 2MB
                    if (memoryStream.Length < 2097152)
                    {
                        var file = new Models.Document()
                        {
                            Name = viewModel.DocumentName,
                            FileSize = memoryStream.Length,
                            UploadDate = DateTime.Now,
                            FileURL = "Database",
                            ProjectId = viewModel.ProjectId,
                            Content = memoryStream.ToArray()
                        };

                        _context.Documents.Add(file);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        // Generate a unique name
                        var fileName = $"{Guid.NewGuid().ToString()}{Path.GetExtension(viewModel.DocumentName)}";

                        // Determine the filePath
                        var filePath = Path.Combine(_hostEnvironment.WebRootPath, "documents", fileName);

                        // Save the file to the fileSystem
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await memoryStream.CopyToAsync(fileStream);
                        }

                        // Save the document details to the database
                        var document = new Models.Document()
                        {
                            Name = viewModel.DocumentName,
                            FileSize = memoryStream.Length,
                            UploadDate = DateTime.Now,
                            FileURL = filePath,
                            ProjectId = viewModel.ProjectId,
                        };

                        _context.Documents.Add(document);
                        await _context.SaveChangesAsync();
                    }
                }

                // Get project documents
                var filteredDocuments = _context.Documents
                    .Where(d => d.ProjectId == viewModel.ProjectId)
                    .Select(d => new DocumentViewModel
                    {
                        Name = d.Name,
                        FileSize = d.FileSize,
                        UploadDate = d.UploadDate,
                    })
                    .ToList();

                var documentUploadViewModel = new DocumentUploadViewModel
                {
                    Documents = filteredDocuments,
                    FileUpload = new BufferedSingleFileUploadDb(),
                    ProjectId = viewModel.ProjectId,
                };

                return View("Index", documentUploadViewModel);
            }
            // Get project documents
            var Documents = _context.Documents
                .Where(d => d.ProjectId == viewModel.ProjectId)
                .Select(d => new DocumentViewModel
                {
                    Name = d.Name,
                    FileSize = d.FileSize,
                    UploadDate = d.UploadDate,
                })
                .ToList();

            var ViewModel = new DocumentUploadViewModel
            {
                Documents = Documents,
                FileUpload = new BufferedSingleFileUploadDb(),
                ProjectId = viewModel.ProjectId,
            };

            return View("Index", ViewModel);
        }
    }
}
