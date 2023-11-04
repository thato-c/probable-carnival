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

        [Required(ErrorMessage = "Company Name is required")]
        [StringLength(15)]
        public string CompanyPhoneNumber { get; set;} = string.Empty;

        [Required(ErrorMessage = "Company Name is required")]
        [EmailAddress]
        public string CompanyEmail { get; set; } = string.Empty;
        public ICollection<LicencePurchase> LicencePurchase { get; set; }
    }
}
