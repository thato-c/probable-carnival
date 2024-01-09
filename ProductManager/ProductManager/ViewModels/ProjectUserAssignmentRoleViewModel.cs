namespace ProductManager.ViewModels
{
    public class ProjectUserAssignmentRoleViewModel
    {
        public int ProjectId { get; set; }

        public int CompanyId { get; set; }
        
        public List<UserAssignmentRoleViewModel> ProjectUserRoles { get; set; }
    }

    public class UserAssignmentRoleViewModel
    {
        public string Username { get; set; }

        public string SelectedRole { get; set; }

        public int AssignmentId { get; set; }
    }
}
