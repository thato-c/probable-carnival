using System.ComponentModel.DataAnnotations;

namespace ProductManager.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [EmailAddress]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public int CompanyId { get; set; }
        public Company Company { get; set; }
    }
}
