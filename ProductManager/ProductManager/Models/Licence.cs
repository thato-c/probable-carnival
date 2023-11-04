using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductManager.Models
{
    public class Licence
    {
        [Key]
        public int LicenceId { get; set; }

        [Required(ErrorMessage = "Licence Name is required")]
        [StringLength(50)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Licence Description is required")]
        [StringLength(100)]
        public string Description { get; set; }

        [Required(ErrorMessage = "Licence Cost is required")]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Cost { get; set; }

        [Required(ErrorMessage = "Licence Duration is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Validity monthss must be greater than or equal to 1")]
        public int ValidityMonths { get; set; } = 1;

        public ICollection<LicencePurchase> LicencePurchase { get; set; }
    }
}
