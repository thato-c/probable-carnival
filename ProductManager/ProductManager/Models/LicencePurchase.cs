﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductManager.Models
{
    public class LicencePurchase
    {
        [Key]
        public int PurchaseId { get; set; }

        [Required(ErrorMessage = "Licence Quantity is required")]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        public DateTime PurchaseDate { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(10, 2)")]
        public decimal TotalCost { get; set; }

        public int CompanyId { get; set; }

        public int LicenceId { get; set; }

        public Company Company { get; set; }
        public Licence Licence { get; set; }
    }
}
