using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductManager.Models
{
    public class LicencePurchase
    {
        [Key]
        public int PurchaseId { get; set; }

        [Required(ErrorMessage = "Licence Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than or equal to 1")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Purchase Date is required")]
        public DateTime PurchaseDate { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(10, 2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Total Cost must be a non-negative value")]
        public decimal TotalCost { get; set; }

        public int CompanyId { get; set; }

        public int LicenceId { get; set; }

        public Company Company { get; set; }
        public Licence Licence { get; set; }
    }
}
