using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace quanlybangiay.Models
{
    [Table("Contacts")]
    public class Contact
    {
        [Key]
        public int ContactId { get; set; }

        [StringLength(100)]
        public string? FirstName { get; set; }

        [StringLength(100)]
        public string? LastName { get; set; }

        [StringLength(150)]
        public string? Email { get; set; }

        [StringLength(200)]
        public string? Subject { get; set; }

        public string? Message { get; set; }

        [StringLength(45)]
        public string? IP { get; set; }

        [StringLength(500)]
        public string? Note { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int? UpdatedBy { get; set; }

        [ForeignKey("UpdatedBy")]
        public User? UpdatedByUser { get; set; }
    }
}
