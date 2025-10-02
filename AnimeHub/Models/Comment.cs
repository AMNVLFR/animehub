using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnimeHub.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }

        public int AnimeId { get; set; }

        public string UserId { get; set; } = string.Empty;

        public string? Content { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("AnimeId")]
        public Anime? Anime { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }
    }
}