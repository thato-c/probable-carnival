using System.ComponentModel.DataAnnotations;

namespace ProductManager.ViewModels
{
    public class UserViewModel
    {
        // Licence Purchase properties
        // Licence purchase properties
        [Required(ErrorMessage = "Licence Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than or equal to 1")]
        public int Quantity { get; set; }

        public int CompanyId { get; set; }

        // User properties
        public List<UserRegistrationViewModel> Users { get; set; }
         

    }
}
