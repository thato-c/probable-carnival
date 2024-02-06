using ProductManager.Models;
using System.ComponentModel.DataAnnotations;

namespace ProductManager.ViewModels
{
    public class DocumentViewModel
    {
        public string Name { get; set; }

        public long FileSize { get; set; }

        public DateTime UploadDate { get; set; }
    }
}
