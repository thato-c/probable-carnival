using ProductManager.Models;

namespace ProductManager.ViewModels
{
    public class DocumentUploadViewModel
    {
        public List<DocumentViewModel> Documents { get; set; }

        public BufferedSingleFileUploadDb FileUpload { get; set; }

        public int ProjectId { get; set; }

    }
}
