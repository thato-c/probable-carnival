using System.ComponentModel.DataAnnotations;

namespace ProductManager.ViewModels
{
    public class CompanyViewModel
    {
        [Required(ErrorMessage = "Company Name is required")]
        [StringLength(50)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Company phone number is required")]
        [StringLength(15)]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Company Email is required")]
        [EmailAddress]
        public string Email { get; set; }
    }
}
