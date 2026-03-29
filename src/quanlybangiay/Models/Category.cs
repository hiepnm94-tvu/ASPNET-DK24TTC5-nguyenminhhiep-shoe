using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace quanlybangiay.Models
{
    [Table("Categories")]
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [StringLength(150)]
        public string CategoryName { get; set; }

        [StringLength(220)]
        public string Slug { get; set; }

        public bool? IsActive { get; set; } = true;

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        [NotMapped]
        public bool IsActiveChecked
        {
            get {
                return IsActive ?? true;
            }
           
            set {
                IsActive = value;
            }
        }
    }
}
