using ProductManager.Models;
using System.ComponentModel.DataAnnotations;

namespace ProductManager.ViewModels
{
    public class DocumentUploadViewModel
    {
        public DocumentUploadViewModel()
        {

            Documents = new List<DocumentViewModel>();
        }
        
        public List<DocumentViewModel> Documents { get; set; }

        public BufferedSingleFileUploadDb FileUpload { get; set; }

        public int ProjectId { get; set; }

        [Required(ErrorMessage = "A file name is required")]
        public string DocumentName { get; set; }

    }
}
