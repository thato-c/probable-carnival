using System.ComponentModel.DataAnnotations;

namespace ProductManager.Models
{
    public class UserProjectRole
    {
        [Key]
        public int ProjectRoleId { get; set; }

        public int AssignmentId { get; set; }

        public int RoleId { get; set; }

        public UserProjectAssignment UserProjectAssignment { get; set; }
        public Role Role { get; set; }
        
    }
}
