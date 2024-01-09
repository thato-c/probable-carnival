using System.ComponentModel.DataAnnotations;

namespace ProductManager.Models
{
    public class UserProjectAssignment
    {
        [Key]
        public int AssignmentId { get; set; }

        public int ProjectId { get; set; }

        public int UserId { get; set; }

        public Project Project { get; set; }

        public User User { get; set; }

        public ICollection<UserProjectRole> UserProjectRoles { get; set; }
    }
}
