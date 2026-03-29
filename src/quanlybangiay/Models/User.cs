using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace quanlybangiay.Models
{
    [Table("Users")]
    public class User
    {
        [Key]
        public int UserId { get; set; }

        public int? RoleId { get; set; }

        [StringLength(100)]
        public string? FullName { get; set; }

        [Required, StringLength(150)]
        public string Email { get; set; }

        [StringLength(255)]
        public string? PasswordHash { get; set; }

        [StringLength(20)]
        public string? Phone { get; set; }

        public byte? Gender { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [StringLength(255)]
        public string? AvatarUrl { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public Role? Role { get; set; }
    }
}
