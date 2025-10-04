using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql;
using AnimeHub.Data;
using AnimeHub.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Add Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Add cookie authentication (Identity handles this, but keeping for compatibility)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();
builder.Services.AddScoped<AnimeHub.Helpers.TmdbService>();

var app = builder.Build();

// Seed the database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        logger.LogInformation("Applying database migrations...");
        context.Database.Migrate();
        logger.LogInformation("Database migrations applied successfully.");

        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        logger.LogInformation("Starting database seeding...");
        SeedData(context, logger);
        SeedAdminUser(userManager, roleManager, logger);
        logger.LogInformation("Database seeding completed.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred seeding the DB.");
        // Continue running the app even if seeding fails
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "animeDetail",
    pattern: "Home/Detail/{title}",
    defaults: new { controller = "Home", action = "Detail" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

static void SeedData(ApplicationDbContext context, ILogger logger)
{
    // Seed Genres
    logger.LogInformation("Seeding genres...");
    var genreNames = new[]
    {
        "Action", "Adventure", "Comedy", "Drama", "Fantasy", "Horror", "Romance",
        "Sci-Fi", "Supernatural", "Psychological", "Superhero", "Thriller",
        "Martial Arts", "School", "Mecha", "Slice of Life", "Mystery",
        "Historical", "Sports", "Isekai"
    };

    foreach (var name in genreNames)
    {
        if (!context.Genres.Any(g => g.Name == name))
        {
            context.Genres.Add(new Genre { Name = name });
            logger.LogInformation("Added genre: {Name}", name);
        }
    }
    context.SaveChanges();
    logger.LogInformation("Genre seeding completed");

    // Seed Anime
    logger.LogInformation("Checking if anime exist...");
    if (!context.Animes.Any())
    {
        logger.LogInformation("Seeding anime...");
        var animes = new[]
        {
            new Anime { Title = "Attack on Titan", Slug = "attack-on-titan", Year = "2013-2023", Rating = 9.2m, Status = "Completed", EpisodeCount = "87", Studio = "MAPPA", CoverUrl = "/images/attack_on_titan.jpg", PosterUrl = "/images/attack_on_titan.jpg", Synopsis = "Humanity fights against giant creatures in a post-apocalyptic world." },
            new Anime { Title = "One Piece", Slug = "one-piece", Year = "1999-Present", Rating = 9.1m, Status = "Ongoing", EpisodeCount = "1000+", Studio = "Toei Animation", CoverUrl = "/images/one_piece.jpg", PosterUrl = "/images/one_piece.jpg", Synopsis = "Follow Luffy's epic adventure to become the Pirate King." },
            new Anime { Title = "Demon Slayer", Slug = "demon-slayer", Year = "2019-2022", Rating = 8.7m, Status = "Completed", EpisodeCount = "44", Studio = "Ufotable", CoverUrl = "/images/demon_slayer.jpg", PosterUrl = "/images/demon_slayer.jpg", Synopsis = "A young boy's journey to save his sister and defeat demons." },
            new Anime { Title = "My Hero Academia", Slug = "my-hero-academia", Year = "2016-Present", Rating = 8.4m, Status = "Ongoing", EpisodeCount = "113", Studio = "Bones", CoverUrl = "/images/my_hero_academia.jpg", PosterUrl = "/images/my_hero_academia.jpg", Synopsis = "Heroes and villains battle in a world where superpowers are common." },
            new Anime { Title = "Naruto", Slug = "naruto", Year = "2002-2007", Rating = 8.3m, Status = "Completed", EpisodeCount = "220", Studio = "Pierrot", CoverUrl = "/images/naruto.jpg", PosterUrl = "/images/naruto.jpg", Synopsis = "A young ninja's journey to become the strongest ninja and leader of his village." },
            new Anime { Title = "Death Note", Slug = "death-note", Year = "2006-2007", Rating = 9.0m, Status = "Completed", EpisodeCount = "37", Studio = "Madhouse", CoverUrl = "/images/death note.jpg", PosterUrl = "/images/death note.jpg", Synopsis = "A student discovers a notebook that allows him to kill anyone by writing their name." },
            new Anime { Title = "Dragon Ball", Slug = "dragon-ball", Year = "1986-1989", Rating = 8.7m, Status = "Completed", EpisodeCount = "153", Studio = "Toei Animation", CoverUrl = "/images/dragon_ball.jpg", PosterUrl = "/images/dragon_ball.jpg", Synopsis = "Goku's adventures in a martial arts world." },
            new Anime { Title = "Dragon Ball Z", Slug = "dragon-ball-z", Year = "1989-1996", Rating = 8.8m, Status = "Completed", EpisodeCount = "291", Studio = "Toei Animation", CoverUrl = "/images/dragon_ball_z.jpg", PosterUrl = "/images/dragon_ball_z.jpg", Synopsis = "Goku defends Earth from powerful villains." },
            new Anime { Title = "Dragon Ball Super", Slug = "dragon-ball-super", Year = "2015-2018", Rating = 7.4m, Status = "Completed", EpisodeCount = "131", Studio = "Toei Animation", CoverUrl = "/images/dragon_ball_super.jpg", PosterUrl = "/images/dragon_ball_super.jpg", Synopsis = "Goku faces new challenges in a more powerful universe." },
            new Anime { Title = "Boruto: Naruto Next Generations", Slug = "boruto-naruto-next-generations", Year = "2017-Present", Rating = 6.9m, Status = "Ongoing", EpisodeCount = "293", Studio = "Pierrot", CoverUrl = "/images/boruto.jpg", PosterUrl = "/images/boruto.jpg", Synopsis = "The next generation of ninjas in the Naruto world." },
            new Anime { Title = "Kaiju No. 8", Slug = "kaiju-no-8", Year = "2024-Present", Rating = 8.5m, Status = "Ongoing", EpisodeCount = "12", Studio = "Production I.G", CoverUrl = "/images/kaiju_no_8.jpg", PosterUrl = "/images/kaiju_no_8.jpg", Synopsis = "A young man fights giant monsters to protect humanity." },
            new Anime { Title = "Jujutsu Kaisen", Slug = "jujutsu-kaisen", Year = "2020-Present", Rating = 8.6m, Status = "Ongoing", EpisodeCount = "24", Studio = "MAPPA", CoverUrl = "/images/jujutsu_kaisen.jpg", PosterUrl = "/images/jujutsu_kaisen.jpg", Synopsis = "Students at a magic school fight cursed spirits." },
            new Anime { Title = "Chainsaw Man", Slug = "chainsaw-man", Year = "2022-Present", Rating = 8.4m, Status = "Ongoing", EpisodeCount = "12", Studio = "MAPPA", CoverUrl = "/images/chainsaw_man.jpg", PosterUrl = "/images/chainsaw_man.jpg", Synopsis = "A devil hunter makes a contract with a chainsaw devil." },
            new Anime { Title = "Attack on Titan: The Final Season", Slug = "attack-on-titan-the-final-season", Year = "2020-2021", Rating = 9.1m, Status = "Completed", EpisodeCount = "16", Studio = "MAPPA", CoverUrl = "/images/attack_on_titan_final.jpg", PosterUrl = "/images/attack_on_titan_final.jpg", Synopsis = "The conclusion of the Attack on Titan story." },
            new Anime { Title = "Fullmetal Alchemist: Brotherhood", Slug = "fullmetal-alchemist-brotherhood", Year = "2009-2010", Rating = 9.1m, Status = "Completed", EpisodeCount = "64", Studio = "Bones", CoverUrl = "/images/full_metal_alchemist_brotherhood.jpg", PosterUrl = "/images/full_metal_alchemist_brotherhood.jpg", Synopsis = "Two brothers search for the Philosopher's Stone." },
            new Anime { Title = "Hunter x Hunter", Slug = "hunter-x-hunter", Year = "2011-2014", Rating = 9.0m, Status = "Completed", EpisodeCount = "148", Studio = "Madhouse", CoverUrl = "/images/hunter_x_hunter.jpg", PosterUrl = "/images/hunter_x_hunter.jpg", Synopsis = "A young boy's journey to become a Hunter." },
            new Anime { Title = "One Punch Man", Slug = "one-punch-man", Year = "2015-Present", Rating = 8.7m, Status = "Ongoing", EpisodeCount = "24", Studio = "Madhouse", CoverUrl = "/images/one_punch_man.jpg", PosterUrl = "/images/one_punch_man.jpg", Synopsis = "A hero who can defeat any opponent with one punch." },
            new Anime { Title = "Tokyo Ghoul", Slug = "tokyo-ghoul", Year = "2014", Rating = 7.8m, Status = "Completed", EpisodeCount = "12", Studio = "Pierrot", CoverUrl = "/images/tokyo_ghoul.jpg", PosterUrl = "/images/tokyo_ghoul.jpg", Synopsis = "A college student becomes a half-ghoul after an accident." },
            new Anime { Title = "Bleach", Slug = "bleach", Year = "2004-2012", Rating = 8.1m, Status = "Completed", EpisodeCount = "366", Studio = "Pierrot", CoverUrl = "/images/bleach.jpg", PosterUrl = "/images/bleach.jpg", Synopsis = "A Soul Reaper protects the living world from evil spirits." },
            new Anime { Title = "Fairy Tail", Slug = "fairy-tail", Year = "2009-2019", Rating = 7.7m, Status = "Completed", EpisodeCount = "328", Studio = "A-1 Pictures", CoverUrl = "/images/fairy_tail.jpg", PosterUrl = "/images/fairy_tail.jpg", Synopsis = "A guild of wizards on magical adventures." }
        };
        context.Animes.AddRange(animes);
        context.SaveChanges();
        logger.LogInformation("Seeded {Count} anime", animes.Length);

        // Seed Anime-Genre relationships
        logger.LogInformation("Seeding anime-genre relationships...");
        var animeGenres = new[]
        {
            // Attack on Titan (1): Action, Drama, Fantasy, Horror
            new AnimeGenre { AnimeId = 1, GenreId = 1 }, new AnimeGenre { AnimeId = 1, GenreId = 4 }, new AnimeGenre { AnimeId = 1, GenreId = 5 }, new AnimeGenre { AnimeId = 1, GenreId = 6 },
            // One Piece (2): Action, Adventure, Comedy, Drama
            new AnimeGenre { AnimeId = 2, GenreId = 1 }, new AnimeGenre { AnimeId = 2, GenreId = 2 }, new AnimeGenre { AnimeId = 2, GenreId = 3 }, new AnimeGenre { AnimeId = 2, GenreId = 4 },
            // Demon Slayer (3): Action, Supernatural, Drama
            new AnimeGenre { AnimeId = 3, GenreId = 1 }, new AnimeGenre { AnimeId = 3, GenreId = 9 }, new AnimeGenre { AnimeId = 3, GenreId = 4 },
            // My Hero Academia (4): Action, Superhero, School
            new AnimeGenre { AnimeId = 4, GenreId = 1 }, new AnimeGenre { AnimeId = 4, GenreId = 11 }, new AnimeGenre { AnimeId = 4, GenreId = 14 },
            // Naruto (5): Action, Adventure, Martial Arts
            new AnimeGenre { AnimeId = 5, GenreId = 1 }, new AnimeGenre { AnimeId = 5, GenreId = 2 }, new AnimeGenre { AnimeId = 5, GenreId = 13 },
            // Death Note (6): Thriller, Psychological, Supernatural
            new AnimeGenre { AnimeId = 6, GenreId = 12 }, new AnimeGenre { AnimeId = 6, GenreId = 10 }, new AnimeGenre { AnimeId = 6, GenreId = 9 },
            // Dragon Ball (7): Action, Adventure, Martial Arts, Comedy
            new AnimeGenre { AnimeId = 7, GenreId = 1 }, new AnimeGenre { AnimeId = 7, GenreId = 2 }, new AnimeGenre { AnimeId = 7, GenreId = 13 }, new AnimeGenre { AnimeId = 7, GenreId = 3 },
            // Dragon Ball Z (8): Action, Adventure, Martial Arts, Superhero
            new AnimeGenre { AnimeId = 8, GenreId = 1 }, new AnimeGenre { AnimeId = 8, GenreId = 2 }, new AnimeGenre { AnimeId = 8, GenreId = 13 }, new AnimeGenre { AnimeId = 8, GenreId = 11 },
            // Dragon Ball Super (9): Action, Adventure, Martial Arts, Superhero
            new AnimeGenre { AnimeId = 9, GenreId = 1 }, new AnimeGenre { AnimeId = 9, GenreId = 2 }, new AnimeGenre { AnimeId = 9, GenreId = 13 }, new AnimeGenre { AnimeId = 9, GenreId = 11 },
            // Boruto (10): Action, Adventure, Martial Arts
            new AnimeGenre { AnimeId = 10, GenreId = 1 }, new AnimeGenre { AnimeId = 10, GenreId = 2 }, new AnimeGenre { AnimeId = 10, GenreId = 13 },
            // Kaiju No. 8 (11): Action, Sci-Fi, Horror, Supernatural
            new AnimeGenre { AnimeId = 11, GenreId = 1 }, new AnimeGenre { AnimeId = 11, GenreId = 8 }, new AnimeGenre { AnimeId = 11, GenreId = 6 }, new AnimeGenre { AnimeId = 11, GenreId = 9 },
            // Jujutsu Kaisen (12): Action, Supernatural, Drama, Horror
            new AnimeGenre { AnimeId = 12, GenreId = 1 }, new AnimeGenre { AnimeId = 12, GenreId = 9 }, new AnimeGenre { AnimeId = 12, GenreId = 4 }, new AnimeGenre { AnimeId = 12, GenreId = 6 },
            // Chainsaw Man (13): Action, Supernatural, Horror, Comedy
            new AnimeGenre { AnimeId = 13, GenreId = 1 }, new AnimeGenre { AnimeId = 13, GenreId = 9 }, new AnimeGenre { AnimeId = 13, GenreId = 6 }, new AnimeGenre { AnimeId = 13, GenreId = 3 },
            // Attack on Titan Final Season (14): Action, Drama, Fantasy, Horror
            new AnimeGenre { AnimeId = 14, GenreId = 1 }, new AnimeGenre { AnimeId = 14, GenreId = 4 }, new AnimeGenre { AnimeId = 14, GenreId = 5 }, new AnimeGenre { AnimeId = 14, GenreId = 6 },
            // Fullmetal Alchemist Brotherhood (15): Action, Adventure, Drama, Fantasy
            new AnimeGenre { AnimeId = 15, GenreId = 1 }, new AnimeGenre { AnimeId = 15, GenreId = 2 }, new AnimeGenre { AnimeId = 15, GenreId = 4 }, new AnimeGenre { AnimeId = 15, GenreId = 5 },
            // Hunter x Hunter (16): Action, Adventure, Fantasy, Martial Arts
            new AnimeGenre { AnimeId = 16, GenreId = 1 }, new AnimeGenre { AnimeId = 16, GenreId = 2 }, new AnimeGenre { AnimeId = 16, GenreId = 5 }, new AnimeGenre { AnimeId = 16, GenreId = 13 },
            // One Punch Man (17): Action, Comedy, Superhero, Sci-Fi
            new AnimeGenre { AnimeId = 17, GenreId = 1 }, new AnimeGenre { AnimeId = 17, GenreId = 3 }, new AnimeGenre { AnimeId = 17, GenreId = 11 }, new AnimeGenre { AnimeId = 17, GenreId = 8 },
            // Tokyo Ghoul (18): Action, Horror, Supernatural, Drama
            new AnimeGenre { AnimeId = 18, GenreId = 1 }, new AnimeGenre { AnimeId = 18, GenreId = 6 }, new AnimeGenre { AnimeId = 18, GenreId = 9 }, new AnimeGenre { AnimeId = 18, GenreId = 4 },
            // Bleach (19): Action, Adventure, Supernatural, Comedy
            new AnimeGenre { AnimeId = 19, GenreId = 1 }, new AnimeGenre { AnimeId = 19, GenreId = 2 }, new AnimeGenre { AnimeId = 19, GenreId = 9 }, new AnimeGenre { AnimeId = 19, GenreId = 3 },
            // Fairy Tail (20): Action, Adventure, Comedy, Fantasy
            new AnimeGenre { AnimeId = 20, GenreId = 1 }, new AnimeGenre { AnimeId = 20, GenreId = 2 }, new AnimeGenre { AnimeId = 20, GenreId = 3 }, new AnimeGenre { AnimeId = 20, GenreId = 5 }
        };
        context.AnimeGenres.AddRange(animeGenres);
        context.SaveChanges();
        logger.LogInformation("Seeded {Count} anime-genre relationships", animeGenres.Length);

        // Seed sample episodes for Dragon Ball Z (always ensure these exist)
        logger.LogInformation("Ensuring Dragon Ball Z episodes exist...");

        // First, find Dragon Ball Z by title to get the correct ID
        var dragonBallZ = context.Animes.FirstOrDefault(a => a.Title == "Dragon Ball Z");
        if (dragonBallZ == null)
        {
            logger.LogWarning("Dragon Ball Z not found in database!");
            return;
        }

        logger.LogInformation("Found Dragon Ball Z with ID: {Id}", dragonBallZ.AnimeId);

        var dbzEpisodes = context.Episodes.Where(e => e.AnimeId == dragonBallZ.AnimeId).ToList();
        logger.LogInformation("Found {Count} existing episodes for Dragon Ball Z", dbzEpisodes.Count);

        var episodesToAdd = new List<Episode>();

        // Episode 1
        if (!dbzEpisodes.Any(e => e.EpisodeNumber == 1))
        {
            logger.LogInformation("Adding Episode 1: The New Threat");
            episodesToAdd.Add(new Episode
            {
                AnimeId = dragonBallZ.AnimeId,
                EpisodeNumber = 1,
                Title = "The New Threat",
                VideoUrl = "https://iframe.mediadelivery.net/play/500620/733bb1e6-4c76-46ce-9fc9-5f70887a5a1e",
                AirDate = new DateTime(1989, 4, 26),
                Duration = "24 min",
                CreatedAt = DateTime.UtcNow
            });
        }

        // Episode 2
        if (!dbzEpisodes.Any(e => e.EpisodeNumber == 2))
        {
            logger.LogInformation("Adding Episode 2: Reunions");
            episodesToAdd.Add(new Episode
            {
                AnimeId = dragonBallZ.AnimeId,
                EpisodeNumber = 2,
                Title = "Reunions",
                VideoUrl = null, // No URL yet
                AirDate = new DateTime(1989, 5, 3),
                Duration = "24 min",
                CreatedAt = DateTime.UtcNow
            });
        }

        if (episodesToAdd.Any())
        {
            context.Episodes.AddRange(episodesToAdd);
            context.SaveChanges();
            logger.LogInformation("Successfully added {Count} Dragon Ball Z episodes", episodesToAdd.Count);
        }
        else
        {
            logger.LogInformation("All Dragon Ball Z episodes already exist");
        }
    
        // Also add episodes for Dragon Ball (animeId 7) to test
        logger.LogInformation("Ensuring Dragon Ball episodes exist...");
        var dragonBall = context.Animes.FirstOrDefault(a => a.Title == "Dragon Ball");
        if (dragonBall != null)
        {
            var dbEpisodes = context.Episodes.Where(e => e.AnimeId == dragonBall.AnimeId).ToList();
            if (!dbEpisodes.Any())
            {
                logger.LogInformation("Adding sample episode for Dragon Ball");
                context.Episodes.Add(new Episode
                {
                    AnimeId = dragonBall.AnimeId,
                    EpisodeNumber = 1,
                    Title = "Secret of the Dragon Balls",
                    VideoUrl = "https://iframe.mediadelivery.net/play/500620/sample-url",
                    AirDate = new DateTime(1986, 2, 26),
                    Duration = "25 min",
                    CreatedAt = DateTime.UtcNow
                });
                context.SaveChanges();
                logger.LogInformation("Added sample episode for Dragon Ball");
            }
        }
    
        // Force add Dragon Ball Z episodes regardless of existing ones (for testing)
        logger.LogInformation("Force adding Dragon Ball Z episodes for testing...");
        try
        {
            // Hardcode Dragon Ball Z ID as 8 based on seeding order
            int dbzAnimeId = 8;
            logger.LogInformation("Using hardcoded Dragon Ball Z ID: {Id}", dbzAnimeId);
    
            // Delete any existing episodes first
            var existingEpisodes = context.Episodes.Where(e => e.AnimeId == dbzAnimeId).ToList();
            if (existingEpisodes.Any())
            {
                context.Episodes.RemoveRange(existingEpisodes);
                context.SaveChanges();
                logger.LogInformation("Removed {Count} existing episodes", existingEpisodes.Count);
            }
    
            // Add fresh episodes
            var forceEpisodes = new[]
            {
                new Episode
                {
                    AnimeId = dbzAnimeId,
                    EpisodeNumber = 1,
                    Title = "The New Threat",
                    VideoUrl = "https://iframe.mediadelivery.net/play/500620/733bb1e6-4c76-46ce-9fc9-5f70887a5a1e",
                    AirDate = new DateTime(1989, 4, 26),
                    Duration = "24 min",
                    CreatedAt = DateTime.UtcNow
                },
                new Episode
                {
                    AnimeId = dbzAnimeId,
                    EpisodeNumber = 2,
                    Title = "Reunions",
                    VideoUrl = null,
                    AirDate = new DateTime(1989, 5, 3),
                    Duration = "24 min",
                    CreatedAt = DateTime.UtcNow
                }
            };
    
            context.Episodes.AddRange(forceEpisodes);
            context.SaveChanges();
            logger.LogInformation("Force added {Count} episodes for Dragon Ball Z", forceEpisodes.Length);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during episode seeding: {Message}", ex.Message);
        }

        // Seed News Articles
        logger.LogInformation("Checking if news articles exist...");
        if (!context.News.Any())
        {
            logger.LogInformation("Seeding news articles...");
            var newsArticles = new[]
            {
                new News
                {
                    Title = "One Piece Chapter 1127: Luffy's Next Big Challenge",
                    Content = @"The latest One Piece chapter has fans buzzing with excitement! In Chapter 1127, Luffy and his Straw Hat crew face their most challenging opponent yet. The chapter reveals shocking new developments about the ancient weapons and the true power of the Pirate King.

Fans are speculating that this new arc will finally bring us closer to the long-awaited climax of the series. The artwork and pacing in this chapter are absolutely phenomenal, showcasing Oda's mastery at storytelling.

What are your thoughts on this latest development? Do you think Luffy is ready for what's coming next?",
                    Author = "AnimeHub Staff",
                    PublishedAt = DateTime.UtcNow.AddDays(-1),
                    ImageUrl = "/images/one_piece.jpg",
                    NewsType = AnimeHub.Models.NewsType.Internal
                },
                new News
                {
                    Title = "Demon Slayer Season 4: Hashira Training Arc Confirmed",
                    Content = @"Great news for Demon Slayer fans! Ufotable has officially confirmed that the Hashira Training Arc will be adapted into Season 4 of the anime. This highly anticipated arc focuses on Tanjiro and his friends undergoing intense training with the Hashira to prepare for the final battle against Muzan Kibutsuji.

The announcement includes details about the production schedule and expected release window. Fans can expect the same high-quality animation that made the series a worldwide phenomenon.

The Hashira Training Arc is considered one of the most emotional and action-packed parts of the manga. We're excited to see how Ufotable brings this story to life on screen!",
                    Author = "AnimeHub Staff",
                    PublishedAt = DateTime.UtcNow.AddDays(-2),
                    ImageUrl = "/images/demon_slayer.jpg",
                    NewsType = AnimeHub.Models.NewsType.Internal
                },
                new News
                {
                    Title = "Jujutsu Kaisen Season 3: Production Update",
                    Content = @"MAPPA has released a new production update for Jujutsu Kaisen Season 3. The studio confirms that production is progressing well and they're aiming for a 2025 release. The season will adapt the Shibuya Incident arc, which is widely regarded as one of the best story arcs in the series.

The update includes concept art and confirms that the voice cast will remain the same. Fans are particularly excited about seeing the intense battles and character development that defined the Shibuya arc in the manga.",
                    Author = "AnimeHub Staff",
                    PublishedAt = DateTime.UtcNow.AddDays(-3),
                    ImageUrl = "/images/jujutsu_kaisen.jpg",
                    NewsType = AnimeHub.Models.NewsType.Internal
                },
                new News
                {
                    Title = "My Hero Academia: Final Season in Production",
                    Content = @"Bones studio has confirmed that My Hero Academia's final season is currently in active production. The season will conclude Izuku Midoriya's journey as he strives to become the greatest hero. Fans can expect emotional payoffs and epic battles as the series reaches its conclusion.

The announcement includes teaser artwork showing the main characters in their final forms. This will be a bittersweet ending for a series that has captivated audiences for over 6 years.",
                    Author = "AnimeHub Staff",
                    PublishedAt = DateTime.UtcNow.AddDays(-4),
                    ImageUrl = "/images/my_hero_academia.jpg",
                    NewsType = AnimeHub.Models.NewsType.Internal
                },
                new News
                {
                    Title = "Chainsaw Man Anime: Season 2 Confirmed",
                    Content = @"MAPPA has officially announced Chainsaw Man Season 2! The second season will adapt the International Assassins arc from Tatsuki Fujimoto's manga. Production is expected to begin in 2025 with a release planned for 2026.

The announcement includes exciting details about the animation style and confirms that the voice cast will return. Fans are eager to see more of Denji's adventures in this unique and action-packed series.",
                    Author = "AnimeHub Staff",
                    PublishedAt = DateTime.UtcNow.AddDays(-5),
                    ImageUrl = "/images/chainsaw_man.jpg",
                    NewsType = AnimeHub.Models.NewsType.Internal
                }
            };
            context.News.AddRange(newsArticles);
            context.SaveChanges();
            logger.LogInformation("Seeded {Count} news articles", newsArticles.Length);
        }
        else
        {
            logger.LogInformation("News articles already exist, skipping seeding");
        }
    }
}

static void SeedAdminUser(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ILogger logger)
{
    // Create Admin role if it doesn't exist
    if (!roleManager.RoleExistsAsync("Admin").Result)
    {
        var roleResult = roleManager.CreateAsync(new IdentityRole("Admin")).Result;
        if (!roleResult.Succeeded)
        {
            logger.LogError("Failed to create Admin role");
            return;
        }
        logger.LogInformation("Created Admin role");
    }

    // Create admin user if it doesn't exist
    var adminUser = userManager.FindByEmailAsync("admin@animehub.com").Result;
    if (adminUser == null)
    {
        adminUser = new ApplicationUser
        {
            UserName = "admin@animehub.com",
            Email = "admin@animehub.com",
            EmailConfirmed = true,
            FirstName = "Admin",
            LastName = "User"
        };

        var userResult = userManager.CreateAsync(adminUser, "Admin123!").Result;
        if (!userResult.Succeeded)
        {
            logger.LogError("Failed to create admin user");
            return;
        }

        var roleAssignResult = userManager.AddToRoleAsync(adminUser, "Admin").Result;
        if (!roleAssignResult.Succeeded)
        {
            logger.LogError("Failed to assign Admin role to user");
            return;
        }

        logger.LogInformation("Created admin user: admin@animehub.com with password: Admin123!");
    }
    else
    {
        logger.LogInformation("Admin user already exists");
    }
}
