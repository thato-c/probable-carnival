namespace ProductManager.ViewModels
{
    public class UserAssignmentRoleViewModel
    {
        public int ProjectId { get; set; }

        public int CompanyId { get; set; }

        public List<AssignmentRoleViewModel> ProjectUserRoles { get; set; }
    }

    public class AssignmentRoleViewModel
    {
        public string Username { get; set; }

        public string SelectedRole { get; set; }

        public int AssignmentId { get; set; }
    }
}
