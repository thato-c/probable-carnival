using System.ComponentModel.DataAnnotations;

namespace ProductManager.Models
{
    public class UserRole
    {
        public int UserRoleId { get; set; }

        [Required(ErrorMessage = "Role is required")]
        [StringLength(50)]
        public int RoleId { get; set; }

        [Required(ErrorMessage = "User is required")]
        public int UserId { get; set; }

        public Role Role { get; set; }

        public User User { get; set; }

    }
}
