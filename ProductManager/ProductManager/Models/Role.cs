using System.ComponentModel.DataAnnotations;

namespace ProductManager.Models
{
    public class Role
    {
        public int RoleId { get; set; }

        [Required(ErrorMessage = "Role name is required")]
        public string RoleName { get; set; }

        public ICollection<UserRole> UserRoles { get; set;}
    }
}
