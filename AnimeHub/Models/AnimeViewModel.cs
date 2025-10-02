using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnimeHub.Models
{
    public class AnimeViewModel
    {
        public int AnimeId { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Slug { get; set; }

        public string? Synopsis { get; set; }

        [StringLength(20)]
        public string? Year { get; set; }

        [Column(TypeName = "decimal(3,1)")]
        public decimal? Rating { get; set; }

        [StringLength(50)]
        public string? Status { get; set; }

        [StringLength(20)]
        public string? EpisodeCount { get; set; }

        [StringLength(100)]
        public string? Studio { get; set; }

        [StringLength(500)]
        public string? CoverUrl { get; set; }

        [StringLength(500)]
        public string? PosterUrl { get; set; }

        [StringLength(500)]
        public string? TrailerUrl { get; set; }

        // Navigation properties
        public ICollection<AnimeGenre> AnimeGenres { get; set; } = new List<AnimeGenre>();

        // User state properties
        public bool IsInFavorites { get; set; }
        public bool IsInWatchlist { get; set; }
        public bool IsBookmarked { get; set; }
    
        // Episode information
        public bool HasEpisodes { get; set; }
        public int? FirstEpisodeId { get; set; }
    }
}