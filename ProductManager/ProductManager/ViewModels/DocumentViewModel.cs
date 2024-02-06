using ProductManager.Models;

namespace ProductManager.ViewModels
{
    public class DocumentViewModel
    {
        public int DocumentId { get; set; }

        public string Name { get; set; }

        public long FileSize { get; set; }

        public DateTime UploadDate { get; set; }
    }
}
