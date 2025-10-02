using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using AnimeHub.Models;

namespace AnimeHub.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Anime> Animes { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<AnimeGenre> AnimeGenres { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<News> News { get; set; }
    public DbSet<ForumPost> ForumPosts { get; set; }
    public DbSet<Watchlist> Watchlists { get; set; }
    public DbSet<Bookmark> Bookmarks { get; set; }
    public DbSet<Favorite> Favorites { get; set; }
    public DbSet<RelatedAnime> RelatedAnimes { get; set; }
    public DbSet<Episode> Episodes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure composite primary keys
        modelBuilder.Entity<AnimeGenre>()
            .HasKey(ag => new { ag.AnimeId, ag.GenreId });

        modelBuilder.Entity<RelatedAnime>()
            .HasKey(ra => new { ra.AnimeId, ra.RelatedAnimeId });

        // Configure self-referencing relationship for RelatedAnime
        modelBuilder.Entity<RelatedAnime>()
            .HasOne(ra => ra.Anime)
            .WithMany(a => a.RelatedAnimes)
            .HasForeignKey(ra => ra.AnimeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<RelatedAnime>()
            .HasOne(ra => ra.Related)
            .WithMany(a => a.RelatedTo)
            .HasForeignKey(ra => ra.RelatedAnimeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
