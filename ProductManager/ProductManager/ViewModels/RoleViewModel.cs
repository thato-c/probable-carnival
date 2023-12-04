using System.ComponentModel.DataAnnotations;

namespace ProductManager.ViewModels
{
    public class RoleViewModel
    {
        [Required(ErrorMessage = "Role name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(100)]
        public string Description { get; set; }
    }
}
