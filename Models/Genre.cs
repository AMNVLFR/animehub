using System.ComponentModel.DataAnnotations;

namespace AnimeHub.Models
{
    public class Genre
    {
        [Key]
        public int GenreId { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        // Navigation property
        public ICollection<AnimeGenre> AnimeGenres { get; set; } = new List<AnimeGenre>();
    }
}