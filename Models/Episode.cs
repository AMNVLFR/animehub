using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnimeHub.Models
{
    public class Episode
    {
        [Key]
        public int EpisodeId { get; set; }

        public int AnimeId { get; set; }

        [Required]
        public int EpisodeNumber { get; set; }

        [StringLength(200)]
        public string? Title { get; set; }

        [StringLength(500)]
        public string? VideoUrl { get; set; }

        public DateTime? AirDate { get; set; }

        [StringLength(10)]
        public string? Duration { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        [ForeignKey("AnimeId")]
        public Anime? Anime { get; set; }
    }
}