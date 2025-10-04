using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AnimeHub.Models;
using AnimeHub.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using AnimeHub.Helpers;

namespace AnimeHub.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly TmdbService _tmdbService;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager, TmdbService tmdbService)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _tmdbService = tmdbService;
    }


    public async Task<IActionResult> Index()
    {
        var popularAnime = await _context.Animes
            .Include(a => a.AnimeGenres).ThenInclude(ag => ag.Genre)
            .OrderByDescending(a => a.Rating)
            .Take(6)
            .ToListAsync();

        _logger.LogInformation("Index action: Found {Count} anime records", popularAnime.Count);
        if (popularAnime.Any())
        {
            _logger.LogInformation("First anime: {Title} with slug: {Slug}", popularAnime.First().Title, popularAnime.First().Slug);
        }

        // Get user state if authenticated
        var user = await _userManager.GetUserAsync(User);
        var animeViewModels = await GetAnimeViewModelsWithUserState(popularAnime, user?.Id);

        _logger.LogInformation("Index action: Returning view with {Count} anime view models", animeViewModels.Count);
        return View(animeViewModels);
    }

    public async Task<IActionResult> Dashboard()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        // Get counts
        ViewBag.WatchlistCount = await _context.Watchlists.CountAsync(w => w.UserId == user.Id);
        ViewBag.FavoritesCount = await _context.Favorites.CountAsync(f => f.UserId == user.Id);
        ViewBag.BookmarksCount = await _context.Bookmarks.CountAsync(b => b.UserId == user.Id);

        // Get favorite anime
        var favoriteAnime = await _context.Favorites
            .Where(f => f.UserId == user.Id)
            .Include(f => f.Anime)
            .OrderByDescending(f => f.AddedAt)
            .Take(4)
            .Select(f => f.Anime)
            .ToListAsync();

        ViewBag.FavoriteAnime = favoriteAnime;

        return View(user);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public async Task<IActionResult> Detail(string title)
    {
        _logger.LogInformation("Detail action called with title: {Title}", title);

        if (string.IsNullOrEmpty(title))
        {
            _logger.LogWarning("Title is null or empty");
            return NotFound();
        }

        var anime = await _context.Animes.FirstOrDefaultAsync(a => a.Slug == title);
        if (anime == null)
        {
            _logger.LogWarning("Anime not found for slug: {Slug}", title);
            return NotFound();
        }

        _logger.LogInformation("Found anime: {Title} with slug: {Slug}", anime.Title, anime.Slug);

        // Fetch TMDB data if TmdbId exists
        if (anime.TmdbId.HasValue)
        {
            _logger.LogInformation("Fetching TMDB data for anime {AnimeId} with TMDB ID {TmdbId}", anime.AnimeId, anime.TmdbId.Value);
            var tmdbDetails = await _tmdbService.GetAnimeDetailsAsync(anime.TmdbId.Value);
            var tmdbImages = await _tmdbService.GetAnimeImagesAsync(anime.TmdbId.Value);

            if (tmdbDetails != null)
            {
                _logger.LogInformation("TMDB details fetched successfully for {TmdbId}: {Name}", anime.TmdbId.Value, tmdbDetails.Name);
            }
            else
            {
                _logger.LogWarning("Failed to fetch TMDB details for {TmdbId}", anime.TmdbId.Value);
            }

            ViewBag.TmdbDetails = tmdbDetails;
            ViewBag.TmdbImages = tmdbImages;
        }
        else
        {
            _logger.LogInformation("No TMDB ID set for anime {AnimeId}", anime.AnimeId);
        }

        return View(anime);
    }

    public async Task<IActionResult> Browse(string? genre = null, string? status = null, string? year = null, string? sort = null, int page = 1)
    {
        ViewData["SelectedGenre"] = genre;
        ViewData["SelectedStatus"] = status;
        ViewData["SelectedYear"] = year;
        ViewData["SelectedSort"] = sort ?? "popularity";
        ViewData["CurrentPage"] = page;
        ViewData["Genres"] = await _context.Genres.OrderBy(g => g.Name).ToListAsync();

        var query = _context.Animes.Include(a => a.AnimeGenres).ThenInclude(ag => ag.Genre).AsQueryable();

        // Genre filter
        if (!string.IsNullOrEmpty(genre))
        {
            query = query.Where(a => a.AnimeGenres.Any(ag => ag.Genre.Name.ToLower() == genre.ToLower()));
        }

        // Status filter
        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(a => a.Status.ToLower() == status.ToLower());
        }

        // Year filter
        if (!string.IsNullOrEmpty(year))
        {
            if (year == "older")
            {
                // Assume older than 2019
                query = query.Where(a => !a.Year.Contains("202") && !a.Year.Contains("2019"));
            }
            else
            {
                query = query.Where(a => a.Year.Contains(year));
            }
        }

        // Sort
        if (!string.IsNullOrEmpty(sort))
        {
            switch (sort.ToLower())
            {
                case "rating":
                    query = query.OrderByDescending(a => a.Rating);
                    break;
                case "year":
                    query = query.OrderByDescending(a => a.Year);
                    break;
                case "alphabetical":
                    query = query.OrderBy(a => a.Title);
                    break;
                case "popularity":
                default:
                    query = query.OrderByDescending(a => a.Rating);
                    break;
            }
        }
        else
        {
            // Default sort by rating
            query = query.OrderByDescending(a => a.Rating);
        }

        const int pageSize = 12;
        var totalCount = await query.CountAsync();
        var animeList = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        ViewData["TotalCount"] = totalCount;
        ViewData["HasMore"] = (page * pageSize) < totalCount;

        // Get user state if authenticated
        var user = await _userManager.GetUserAsync(User);
        var animeViewModels = await GetAnimeViewModelsWithUserState(animeList, user?.Id);

        return View(animeViewModels);
    }

    [HttpGet]
    public async Task<IActionResult> GetAnimeGrid(string? genre = null, string? status = null, string? year = null, string? sort = null, int page = 1)
    {
        var query = _context.Animes.Include(a => a.AnimeGenres).ThenInclude(ag => ag.Genre).AsQueryable();

        // Genre filter
        if (!string.IsNullOrEmpty(genre))
        {
            query = query.Where(a => a.AnimeGenres.Any(ag => ag.Genre.Name.ToLower() == genre.ToLower()));
        }

        // Status filter
        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(a => a.Status.ToLower() == status.ToLower());
        }

        // Year filter
        if (!string.IsNullOrEmpty(year))
        {
            if (year == "older")
            {
                query = query.Where(a => !a.Year.Contains("202") && !a.Year.Contains("2019"));
            }
            else
            {
                query = query.Where(a => a.Year.Contains(year));
            }
        }

        // Sort
        if (!string.IsNullOrEmpty(sort))
        {
            switch (sort.ToLower())
            {
                case "rating":
                    query = query.OrderByDescending(a => a.Rating);
                    break;
                case "year":
                    query = query.OrderByDescending(a => a.Year);
                    break;
                case "alphabetical":
                    query = query.OrderBy(a => a.Title);
                    break;
                case "popularity":
                default:
                    query = query.OrderByDescending(a => a.Rating);
                    break;
            }
        }
        else
        {
            query = query.OrderByDescending(a => a.Rating);
        }

        const int pageSize = 12;
        var animeList = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        // Get user state if authenticated
        var user = await _userManager.GetUserAsync(User);
        var animeViewModels = await GetAnimeViewModelsWithUserState(animeList, user?.Id);

        return PartialView("_AnimeGrid", animeViewModels);
    }

    [HttpGet]
    public async Task<IActionResult> Search(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return RedirectToAction("Index");
        }

        ViewData["SearchQuery"] = query;

        var searchResults = await _context.Animes
            .Include(a => a.AnimeGenres)
            .ThenInclude(ag => ag.Genre)
            .Where(a =>
                a.Title.ToLower().Contains(query.ToLower()) ||
                (a.Synopsis != null && a.Synopsis.ToLower().Contains(query.ToLower())) ||
                a.AnimeGenres.Any(ag => ag.Genre.Name.ToLower().Contains(query.ToLower())))
            .ToListAsync();

        return View(searchResults);
    }

    public IActionResult Genres()
    {
        return View();
    }

    public async Task<IActionResult> News()
    {
        var news = await _context.News
            .OrderByDescending(n => n.PublishedAt)
            .Take(10) // Show latest 10 news articles
            .ToListAsync();

        return View(news);
    }

    [HttpPost]
    public async Task<IActionResult> AddToWatchlist(int animeId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Json(new { success = false });

        var existing = await _context.Watchlists.FirstOrDefaultAsync(w => w.UserId == user.Id && w.AnimeId == animeId);
        if (existing != null) return Json(new { success = false, message = "Already in watchlist" });

        _context.Watchlists.Add(new Watchlist { UserId = user.Id, AnimeId = animeId });
        await _context.SaveChangesAsync();
        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> RemoveFromWatchlist(int animeId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Json(new { success = false });

        var item = await _context.Watchlists.FirstOrDefaultAsync(w => w.UserId == user.Id && w.AnimeId == animeId);
        if (item == null) return Json(new { success = false });

        _context.Watchlists.Remove(item);
        await _context.SaveChangesAsync();
        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> AddToBookmarks(int animeId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Json(new { success = false });

        var existing = await _context.Bookmarks.FirstOrDefaultAsync(b => b.UserId == user.Id && b.AnimeId == animeId);
        if (existing != null) return Json(new { success = false, message = "Already bookmarked" });

        _context.Bookmarks.Add(new Bookmark { UserId = user.Id, AnimeId = animeId });
        await _context.SaveChangesAsync();
        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> RemoveFromBookmarks(int animeId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Json(new { success = false });

        var item = await _context.Bookmarks.FirstOrDefaultAsync(b => b.UserId == user.Id && b.AnimeId == animeId);
        if (item == null) return Json(new { success = false });

        _context.Bookmarks.Remove(item);
        await _context.SaveChangesAsync();
        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> AddToFavorites(int animeId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Json(new { success = false });

        var existing = await _context.Favorites.FirstOrDefaultAsync(f => f.UserId == user.Id && f.AnimeId == animeId);
        if (existing != null) return Json(new { success = false, message = "Already favorited" });

        _context.Favorites.Add(new Favorite { UserId = user.Id, AnimeId = animeId });
        await _context.SaveChangesAsync();
        return Json(new { success = true });
    }

    // Comment API Endpoints
    [HttpGet]
    public async Task<IActionResult> GetCommentCount(int animeId)
    {
        var count = await _context.Comments.CountAsync(c => c.AnimeId == animeId);
        return Json(new { count });
    }

    [HttpGet]
    public async Task<IActionResult> GetComments(int animeId, int page = 1, int pageSize = 10)
    {
        var comments = await _context.Comments
            .Where(c => c.AnimeId == animeId)
            .Include(c => c.User)
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new
            {
                c.CommentId,
                c.Content,
                c.CreatedAt,
                UserName = c.User != null ? c.User.UserName : "Anonymous",
                UserAvatar = (string?)null
            })
            .ToListAsync();

        var totalCount = await _context.Comments.CountAsync(c => c.AnimeId == animeId);
        var hasMore = (page * pageSize) < totalCount;

        return Json(new { comments, hasMore, totalCount });
    }

    [HttpPost]
    public async Task<IActionResult> AddComment(int animeId, string content)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Json(new { success = false, message = "Not authenticated" });

        if (string.IsNullOrWhiteSpace(content))
            return Json(new { success = false, message = "Comment cannot be empty" });

        var comment = new Comment
        {
            AnimeId = animeId,
            UserId = user.Id,
            Content = content.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        return Json(new
        {
            success = true,
            comment = new
            {
                comment.CommentId,
                comment.Content,
                comment.CreatedAt,
                UserName = user.UserName,
                UserAvatar = (string?)null
            }
        });
    }

    // Episode API Endpoints
    [HttpGet]
    public async Task<IActionResult> GetEpisodes(int animeId)
    {
        _logger.LogInformation("GetEpisodes called with animeId: {AnimeId}", animeId);

        var episodes = await _context.Episodes
            .Where(e => e.AnimeId == animeId)
            .OrderBy(e => e.EpisodeNumber)
            .Select(e => new
            {
                e.EpisodeId,
                e.EpisodeNumber,
                e.Title,
                e.VideoUrl,
                e.AirDate,
                e.Duration
            })
            .ToListAsync();

        _logger.LogInformation("Found {Count} episodes for animeId {AnimeId}", episodes.Count, animeId);

        return Json(episodes);
    }

    [HttpGet]
    public async Task<IActionResult> WatchEpisode(int episodeId)
    {
        var episode = await _context.Episodes
            .Include(e => e.Anime)
            .FirstOrDefaultAsync(e => e.EpisodeId == episodeId);

        if (episode == null) return NotFound();

        return View(episode);
    }

    [HttpPost]
    public async Task<IActionResult> RemoveFromFavorites(int animeId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Json(new { success = false });

        var item = await _context.Favorites.FirstOrDefaultAsync(f => f.UserId == user.Id && f.AnimeId == animeId);
        if (item == null) return Json(new { success = false });

        _context.Favorites.Remove(item);
        await _context.SaveChangesAsync();
        return Json(new { success = true });
    }
    public IActionResult About()
    {
        return View();
    }

    public async Task<IActionResult> Bookmarks()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var bookmarks = await _context.Bookmarks
            .Where(b => b.UserId == user.Id)
            .Include(b => b.Anime)
                .ThenInclude(a => a.AnimeGenres)
                .ThenInclude(ag => ag.Genre)
            .OrderByDescending(b => b.AddedAt)
            .ToListAsync();

        return View(bookmarks);
    }

    public async Task<IActionResult> Favorites()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var favorites = await _context.Favorites
            .Where(f => f.UserId == user.Id)
            .Include(f => f.Anime)
                .ThenInclude(a => a.AnimeGenres)
                .ThenInclude(ag => ag.Genre)
            .OrderByDescending(f => f.AddedAt)
            .ToListAsync();

        return View(favorites);
    }

    public async Task<IActionResult> Watchlist()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var watchlist = await _context.Watchlists
            .Where(w => w.UserId == user.Id)
            .Include(w => w.Anime)
                .ThenInclude(a => a.AnimeGenres)
                .ThenInclude(ag => ag.Genre)
            .OrderByDescending(w => w.AddedAt)
            .ToListAsync();

        return View(watchlist);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    private async Task<List<AnimeViewModel>> GetAnimeViewModelsWithUserState(List<Anime> animes, string? userId)
    {
        var animeIds = animes.Select(a => a.AnimeId).ToList();

        // Get episode information for all animes
        var episodeInfo = await _context.Episodes
            .Where(e => animeIds.Contains(e.AnimeId))
            .GroupBy(e => e.AnimeId)
            .Select(g => new
            {
                AnimeId = g.Key,
                HasEpisodes = g.Any(),
                FirstEpisodeId = g.OrderBy(e => e.EpisodeNumber).Select(e => e.EpisodeId).FirstOrDefault()
            })
            .ToDictionaryAsync(x => x.AnimeId, x => new { x.HasEpisodes, x.FirstEpisodeId });

        var viewModels = animes.Select(a => new AnimeViewModel
        {
            AnimeId = a.AnimeId,
            Title = a.Title,
            Slug = a.Slug,
            Synopsis = a.Synopsis,
            Year = a.Year,
            Rating = a.Rating,
            Status = a.Status,
            EpisodeCount = a.EpisodeCount,
            Studio = a.Studio,
            CoverUrl = a.CoverUrl,
            PosterUrl = a.PosterUrl,
            TrailerUrl = a.TrailerUrl,
            AnimeGenres = a.AnimeGenres,
            IsInFavorites = false,
            IsInWatchlist = false,
            IsBookmarked = false,
            HasEpisodes = episodeInfo.ContainsKey(a.AnimeId) && episodeInfo[a.AnimeId].HasEpisodes,
            FirstEpisodeId = episodeInfo.ContainsKey(a.AnimeId) ? episodeInfo[a.AnimeId].FirstEpisodeId : null
        }).ToList();

        if (!string.IsNullOrEmpty(userId))
        {
            // Get user's favorites
            var favoriteIds = await _context.Favorites
                .Where(f => f.UserId == userId && animeIds.Contains(f.AnimeId))
                .Select(f => f.AnimeId)
                .ToListAsync();

            // Get user's watchlist
            var watchlistIds = await _context.Watchlists
                .Where(w => w.UserId == userId && animeIds.Contains(w.AnimeId))
                .Select(w => w.AnimeId)
                .ToListAsync();

            // Get user's bookmarks
            var bookmarkIds = await _context.Bookmarks
                .Where(b => b.UserId == userId && animeIds.Contains(b.AnimeId))
                .Select(b => b.AnimeId)
                .ToListAsync();

            // Update view models with user state
            foreach (var vm in viewModels)
            {
                vm.IsInFavorites = favoriteIds.Contains(vm.AnimeId);
                vm.IsInWatchlist = watchlistIds.Contains(vm.AnimeId);
                vm.IsBookmarked = bookmarkIds.Contains(vm.AnimeId);
            }
        }

        return viewModels;
    }

    private async Task<int?> GetFirstEpisodeId(int animeId)
    {
        var firstEpisode = await _context.Episodes
            .Where(e => e.AnimeId == animeId)
            .OrderBy(e => e.EpisodeNumber)
            .FirstOrDefaultAsync();

        return firstEpisode?.EpisodeId;
    }
}
