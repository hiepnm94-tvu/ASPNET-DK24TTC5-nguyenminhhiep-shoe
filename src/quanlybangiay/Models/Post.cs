using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace quanlybangiay.Models
{
    [Table("Posts")]
    public class Post
    {
        [Key]
        public int PostId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(220)]
        public string? Slug { get; set; }

        [StringLength(500)]
        public string? ShortDescription { get; set; }

        public string? Content { get; set; }

        [StringLength(255)]
        public string? ThumbnailUrl { get; set; }

        public bool? IsActive { get; set; } = true;

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public int? CreatedBy { get; set; }

        public int? UpdatedBy { get; set; }

        [ForeignKey("CreatedBy")]
        public User? CreatedByUser { get; set; }

        [ForeignKey("UpdatedBy")]
        public User? UpdatedByUser { get; set; }
    }
}
