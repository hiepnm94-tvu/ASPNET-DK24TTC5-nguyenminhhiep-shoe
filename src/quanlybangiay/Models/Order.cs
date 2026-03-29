using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace quanlybangiay.Models
{
    [Table("Orders")]
    public class Order
    {
        [Key]
        public long OrderId { get; set; }

        [Required, StringLength(20)]
        public string OrderCode { get; set; }

        public int UserId { get; set; }

        public int AddressId { get; set; }

        public int? PromotionId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Subtotal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ShippingFee { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        public byte? PaymentStatus { get; set; }

        public byte? OrderStatus { get; set; }

        [StringLength(500)]
        public string Note { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public User User { get; set; }
    }
}
