// User Assignment View Model
using System.ComponentModel.DataAnnotations;

namespace ProductManager.ViewModels
{
    public class UserAssignmentViewModel
    {
        public int CompanyId { get; set; }

        public int ProjectId { get; set; }
    }
}
