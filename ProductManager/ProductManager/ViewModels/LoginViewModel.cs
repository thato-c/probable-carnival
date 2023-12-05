using System.ComponentModel.DataAnnotations;

namespace ProductManager.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username is required")]
        [EmailAddress]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
