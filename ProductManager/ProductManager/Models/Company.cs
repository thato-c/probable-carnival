using System.ComponentModel.DataAnnotations;

namespace ProductManager.Models
{
    public class Company
    {
        [Key]
        public int CompanyId { get; set; }

        [Required(ErrorMessage = "Company Name is required")]
        [StringLength(50)]
        public string CompanyName { get; set; }

        [Required(ErrorMessage = "Company Phone Number is required")]
        [StringLength(10, ErrorMessage = "Phone number must be at least 10 characters in length.", MinimumLength = 10)]
        public string CompanyPhoneNumber { get; set;} = string.Empty;

        [Required(ErrorMessage = "Company Email is required")]
        [EmailAddress]
        public string CompanyEmail { get; set; } = string.Empty;

        public string Payment { get; set; } = "Unpaid";

        [Required(ErrorMessage = "Admin Email is required")]
        [EmailAddress]
        public string AdminEmail { get; set; }

        [Timestamp]
        public byte RowVersion { get; set; }

        public ICollection<LicencePurchase> LicencePurchases { get; set; }
        public ICollection<User> Users { get; set; }

        public ICollection<Project> Projects { get; set; }
    }
}
