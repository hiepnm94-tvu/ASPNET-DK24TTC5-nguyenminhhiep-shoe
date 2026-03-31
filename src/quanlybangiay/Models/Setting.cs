using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace quanlybangiay.Models
{
    [Table("Settings")]
    public class Setting
    {
        [Key]
        public int SettingId { get; set; }

        [StringLength(200)]
        public string? ShopName { get; set; }

        [StringLength(255)]
        public string? LogoUrl { get; set; }

        [StringLength(20)]
        public string? Phone { get; set; }

        [StringLength(20)]
        public string? Hotline { get; set; }

        [StringLength(150)]
        public string? Email { get; set; }

        [StringLength(500)]
        public string? Address { get; set; }

        [StringLength(500)]
        public string? WorkingHours { get; set; }

        public string? FooterContent { get; set; }

        [StringLength(255)]
        public string? CopyrightText { get; set; }

        [StringLength(255)]
        public string? FacebookUrl { get; set; }

        [StringLength(255)]
        public string? InstagramUrl { get; set; }

        [StringLength(255)]
        public string? YoutubeUrl { get; set; }

        [StringLength(255)]
        public string? TiktokUrl { get; set; }

        [StringLength(255)]
        public string? ZaloUrl { get; set; }

        [StringLength(200)]
        public string? MetaTitle { get; set; }

        [StringLength(500)]
        public string? MetaDescription { get; set; }

        [StringLength(500)]
        public string? MetaKeywords { get; set; }

        [StringLength(255)]
        public string? FaviconUrl { get; set; }

        [StringLength(255)]
        public string? BannerUrl { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public int? CreatedBy { get; set; }

        public int? UpdatedBy { get; set; }

        [ForeignKey("CreatedBy")]
        public User? CreatedByUser { get; set; }

        [ForeignKey("UpdatedBy")]
        public User? UpdatedByUser { get; set; }
    }
}
