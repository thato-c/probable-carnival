using System.ComponentModel.DataAnnotations;

namespace ProductManager.Models
{
    public class BufferedSingleFileUploadDb
    {
        [Required]
        [Display(Name = "File")]
        public IFormFile FormFile { get; set; }
    }
}
