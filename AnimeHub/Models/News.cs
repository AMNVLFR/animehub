using System.ComponentModel.DataAnnotations;

namespace AnimeHub.Models
{
    public enum NewsType
    {
        External,  // Links to external websites
        Internal   // Creates forum discussion
    }

    public class News
    {
        [Key]
        public int NewsId { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; } = string.Empty;

        public string? Content { get; set; }

        [StringLength(100)]
        public string? Author { get; set; }

        [StringLength(500)]
        public string? ImageUrl { get; set; }

        public NewsType NewsType { get; set; } = NewsType.Internal;

        [StringLength(1000)]
        public string? LinkUrl { get; set; }

        public DateTime PublishedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties for forum
        public ICollection<ForumPost> ForumPosts { get; set; } = new List<ForumPost>();
    }
}