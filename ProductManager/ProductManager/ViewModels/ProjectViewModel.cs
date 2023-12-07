using System.ComponentModel.DataAnnotations;

namespace ProductManager.ViewModels
{
    public class ProjectViewModel
    {
        [Required(ErrorMessage = "Project name is required.")]
        [StringLength(100)]
        public string Name { get; set; }

        public int CompanyId { get; set; }
    }
}
