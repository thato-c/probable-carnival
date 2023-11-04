using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductManager.Models
{
    public class LicencePurchase
    {
        [Key]
        public int PurchaseId { get; set; }

        public int Quantity { get; set; }

        public DateTime PurchaseDate { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal TotalCost { get; set; }

        public int CompanyId { get; set; }

        public int LicenceId { get; set; }

        public Company Company { get; set; }
        public Licence Licence { get; set; }
    }
}
