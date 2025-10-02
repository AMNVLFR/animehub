using System.ComponentModel.DataAnnotations;

namespace AnimeHub.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        [StringLength(500)]
        public string? AvatarUrl { get; set; }

        // Navigation properties
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Watchlist> Watchlists { get; set; } = new List<Watchlist>();
    }
}