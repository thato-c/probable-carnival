using System.ComponentModel.DataAnnotations;

namespace ProductManager.ViewModels
{
    public class ProjectUserAssignmentViewModel
    {
        public int ProjectId { get; set; }

        public int CompanyId { get; set; }

        public List<ProjectUserViewModel> ProjectUsers { get; set; }

        [Required(ErrorMessage = "No users have been selected.")]
        public string CheckedUsernames { get; set; }
    }

    public class ProjectUserViewModel
    {
        public string Username { get; set; }
        public bool IsSelected { get; set; }
    }
}
