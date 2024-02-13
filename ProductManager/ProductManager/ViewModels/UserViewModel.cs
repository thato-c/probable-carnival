using System.ComponentModel.DataAnnotations;

namespace ProductManager.ViewModels
{
    public class UserViewModel
    {
        public string  CompanyName { get; set; }

        // User properties
        public List<UserRegistrationViewModel> Users { get; set; }
    }
}
