using System.ComponentModel.DataAnnotations.Schema;

namespace AnimeHub.Models
{
    public class AnimeGenre
    {
        public int AnimeId { get; set; }

        public int GenreId { get; set; }

        // Navigation properties
        [ForeignKey("AnimeId")]
        public Anime? Anime { get; set; }

        [ForeignKey("GenreId")]
        public Genre? Genre { get; set; }
    }
}