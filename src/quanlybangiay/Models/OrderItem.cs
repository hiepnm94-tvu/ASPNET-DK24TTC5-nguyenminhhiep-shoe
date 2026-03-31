using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace quanlybangiay.Models
{
    [Table("OrderItems")]
    public class OrderItem
    {
        [Key]
        public long OrderItemId { get; set; }

        public long OrderId { get; set; }

        public int VariantId { get; set; }

        [StringLength(200)]
        public string ProductName { get; set; } = string.Empty;

        [StringLength(10)]
        public string? SizeValue { get; set; }

        [StringLength(50)]
        public string? ColorName { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal LineTotal { get; set; }

        // Navigation
        public Order? Order { get; set; }
        public ProductVariant? Variant { get; set; }
    }
}
