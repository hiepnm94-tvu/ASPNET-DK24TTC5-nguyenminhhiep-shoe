using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace quanlybangiay.Models
{
    [Table("Products")]
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        public int CategoryId { get; set; }

        [StringLength(200)]
        public string ProductName { get; set; }

        [Required, StringLength(220)]
        public string Slug { get; set; }

        [StringLength(100)]
        public string Brand { get; set; }

        [StringLength(500)]
        public string ShortDescription { get; set; }

        public string Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal BasePrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? DiscountPrice { get; set; }

        [StringLength(255)]
        public string ThumbnailUrl { get; set; }

        public byte? Status { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public Category Category { get; set; }
    }
}
