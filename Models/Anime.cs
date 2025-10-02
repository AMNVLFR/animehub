using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnimeHub.Models
{
    public class Anime
    {
        [Key]
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
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Watchlist> Watchlists { get; set; } = new List<Watchlist>();
        public ICollection<RelatedAnime> RelatedAnimes { get; set; } = new List<RelatedAnime>();
        public ICollection<RelatedAnime> RelatedTo { get; set; } = new List<RelatedAnime>();
        public ICollection<Episode> Episodes { get; set; } = new List<Episode>();
    }
}