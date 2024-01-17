using System.ComponentModel.DataAnnotations;

namespace ProductManager.ViewModels
{
    public class RegistrationViewModel
    {
        [Required(ErrorMessage = "Company Name is required")]
        [StringLength(50)]
        public string CompanyName { get; set; }

        [Required(ErrorMessage = "Company phone number is required")]
        [StringLength(10, ErrorMessage = "Phone number must be at least 10 characters in length.", MinimumLength = 10)]
        public string CompanyPhoneNumber { get; set; }

        [Required(ErrorMessage = "Company Email is required")]
        [EmailAddress]
        public string CompanyEmail { get; set; }

        [Required(ErrorMessage = "The Administrator's email is required")]
        [EmailAddress]
        public string AdminEmail { get; set; }
    }
}
