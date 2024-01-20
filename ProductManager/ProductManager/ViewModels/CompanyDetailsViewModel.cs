using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProductManager.ViewModels
{
    public class CompanyDetailsViewModel
    {
        // Compnay properties
        [Required(ErrorMessage = "Company Name is required")]
        [StringLength(50)]
        public string CompanyName { get; set; }

        [Required(ErrorMessage = "Company phone number is required")]
        [StringLength(10, ErrorMessage = "Phone number must be at least 10 characters in length.", MinimumLength = 10)]
        public string CompanyPhoneNumber { get; set; }

        [Required(ErrorMessage = "Company Email is required")]
        [EmailAddress]
        public string CompanyEmail { get; set; }

        // Admin properties
        [Required(ErrorMessage = "Admin Email is required")]
        [EmailAddress]
        public string AdminEmail { get; set; }

        // Company Licencing Properties
        [Required(ErrorMessage = "Licence name is required")]
        public string LicenceName { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        [Required(ErrorMessage = "Licence Cost is required")]
        public decimal LicenceCost { get; set; }

        [Required(ErrorMessage = "Licence Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than or equal to 1")]
        public int Quantity { get; set; }

        [Display(Name = "Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Purchase Date is required")]
        public DateTime PurchaseDate { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(10, 2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Total Cost must be a non-negative value")]
        public decimal TotalCost { get; set; }
    }
}
