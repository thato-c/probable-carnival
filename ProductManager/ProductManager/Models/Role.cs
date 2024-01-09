using System.ComponentModel.DataAnnotations;

namespace ProductManager.Models
{
    public class Role
    {
        public int RoleId { get; set; }

        [Required(ErrorMessage = "Role name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(100)]
        public string Description { get; set; }

        public ICollection<UserRole> UserRoles { get; set;}

        public ICollection<UserProjectRole> UserProjectRoles { get; set; }
    }
}
