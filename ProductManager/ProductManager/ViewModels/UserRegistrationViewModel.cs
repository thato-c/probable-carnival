using System.ComponentModel.DataAnnotations;

namespace ProductManager.ViewModels
{
    public class UserRegistrationViewModel
    {
        [Required(ErrorMessage = "Username is required")]
        [EmailAddress]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", 
            ErrorMessage = "Password must meet the recommended strength.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "User Role is required")]
        public string SelectedRole { get; set; }

        public IEnumerable<LicenceDropDownItem> Roles { get; set; }

        public int UserId { get; set; }
    }
}
