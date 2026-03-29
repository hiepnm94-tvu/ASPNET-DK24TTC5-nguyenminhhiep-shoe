using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace quanlybangiay.Models
{
    [Table("ProductVariants")]
    public class ProductVariant
    {
        [Key]
        public int VariantId { get; set; }

        public int ProductId { get; set; }

        [Required, StringLength(50)]
        public string SKU { get; set; }

        [StringLength(10)]
        public string SizeValue { get; set; }

        [StringLength(50)]
        public string ColorName { get; set; }

        [StringLength(10)]
        public string ColorCode { get; set; }

        public int StockQty { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? AdditionalPrice { get; set; }

        public int? WeightGram { get; set; }

        public bool IsDefault { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Product Product { get; set; }
    }
}
