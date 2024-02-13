using System.ComponentModel.DataAnnotations;

namespace ProductManager.ViewModels
{
    public class UserViewModel
    {
        // User properties
        public List<UserRegistrationViewModel> Users { get; set; }
    }
}
