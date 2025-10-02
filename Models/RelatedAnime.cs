using System.ComponentModel.DataAnnotations.Schema;

namespace AnimeHub.Models
{
    public class RelatedAnime
    {
        public int AnimeId { get; set; }

        public int RelatedAnimeId { get; set; }

        // Navigation properties
        [ForeignKey("AnimeId")]
        public Anime? Anime { get; set; }

        [ForeignKey("RelatedAnimeId")]
        public Anime? Related { get; set; }
    }
}