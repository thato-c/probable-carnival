using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductManager.ViewModels
{
    public class LicencePurchaseViewModel
    {
        public CompanyViewModel CompanyDetails { get; set; }
        public LicenceViewModel LicenceDetails { get; set; }

        // Licence purchase information
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
