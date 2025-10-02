using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnimeHub.Models
{
    public class Favorite
    {
        [Key]
        public int FavoriteId { get; set; }

        public string UserId { get; set; } = string.Empty;

        public int AnimeId { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }

        [ForeignKey("AnimeId")]
        public Anime? Anime { get; set; }
    }
}