using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductManager.ViewModels
{
    public class LicenceViewModel
    {
        [Required(ErrorMessage = "Licence name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Licence Description is required")]
        public string Description { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        [Required(ErrorMessage = "Licence Cost is required")]
        public decimal Cost { get; set; }

        [Range(1, int.MaxValue)]
        [Required(ErrorMessage = "Licence Duration is required")]
        public int ValidityMonths { get; set; } = 1;
    }
}