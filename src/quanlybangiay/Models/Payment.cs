using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace quanlybangiay.Models
{
    [Table("Payments")]
    public class Payment
    {
        [Key]
        public long PaymentId { get; set; }

        public long OrderId { get; set; }

        [StringLength(30)]
        public string? Method { get; set; }

        [StringLength(100)]
        public string? GatewayTransactionId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public DateTime? PaidAt { get; set; }

        public byte? Status { get; set; }

        public string? ResponsePayload { get; set; }

        // Navigation
        public Order? Order { get; set; }
    }
}
