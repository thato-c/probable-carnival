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
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", 
            ErrorMessage = "Password must meet the recommended strength.")]
        public string Password { get; set; }

        public int CompanyId { get; set; }
        public Company Company { get; set; }

        public ICollection<UserRole> UserRoles { get; set; }

        public ICollection<UserProjectAssignment> ProjectAssignments { get; set; }
    }
}
