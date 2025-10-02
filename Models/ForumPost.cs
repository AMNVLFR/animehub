using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnimeHub.Models
{
    public class ForumPost
    {
        [Key]
        public int ForumPostId { get; set; }

        public int NewsId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey("NewsId")]
        public News? News { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }

        // For threaded replies
        public int? ParentPostId { get; set; }

        [ForeignKey("ParentPostId")]
        public ForumPost? ParentPost { get; set; }

        public ICollection<ForumPost> Replies { get; set; } = new List<ForumPost>();
    }
}