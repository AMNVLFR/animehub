using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AnimeHub.Data;
using AnimeHub.Models;

namespace AnimeHub.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
    
        public AdminController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: Admin
        public IActionResult Index()
        {
            return View();
        }

        // GET: Admin/Episodes
        public async Task<IActionResult> Episodes()
        {
            var episodes = await _context.Episodes
                .Include(e => e.Anime)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();
            return View(episodes);
        }

        // GET: Admin/Episodes/Create
        public IActionResult CreateEpisode()
        {
            ViewData["AnimeId"] = new SelectList(_context.Animes.OrderBy(a => a.Title), "AnimeId", "Title");
            return View();
        }

        // POST: Admin/Episodes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEpisode([Bind("AnimeId,EpisodeNumber,Title,VideoUrl,AirDate,Duration")] Episode episode)
        {
            if (ModelState.IsValid)
            {
                episode.CreatedAt = DateTime.UtcNow;
                _context.Add(episode);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Episode created successfully!";
                return RedirectToAction(nameof(Episodes));
            }
            ViewData["AnimeId"] = new SelectList(_context.Animes.OrderBy(a => a.Title), "AnimeId", "Title", episode.AnimeId);
            return View(episode);
        }

        // GET: Admin/Episodes/Edit/5
        public async Task<IActionResult> EditEpisode(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var episode = await _context.Episodes.FindAsync(id);
            if (episode == null)
            {
                return NotFound();
            }
            ViewData["AnimeId"] = new SelectList(_context.Animes.OrderBy(a => a.Title), "AnimeId", "Title", episode.AnimeId);
            return View(episode);
        }

        // POST: Admin/Episodes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEpisode(int id, [Bind("EpisodeId,AnimeId,EpisodeNumber,Title,VideoUrl,AirDate,Duration,CreatedAt")] Episode episode)
        {
            if (id != episode.EpisodeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(episode);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Episode updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EpisodeExists(episode.EpisodeId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Episodes));
            }
            ViewData["AnimeId"] = new SelectList(_context.Animes.OrderBy(a => a.Title), "AnimeId", "Title", episode.AnimeId);
            return View(episode);
        }

        // GET: Admin/Episodes/Delete/5
        public async Task<IActionResult> DeleteEpisode(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var episode = await _context.Episodes
                .Include(e => e.Anime)
                .FirstOrDefaultAsync(m => m.EpisodeId == id);
            if (episode == null)
            {
                return NotFound();
            }

            return View(episode);
        }

        // POST: Admin/Episodes/Delete/5
        [HttpPost, ActionName("DeleteEpisode")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteEpisodeConfirmed(int id)
        {
            var episode = await _context.Episodes.FindAsync(id);
            if (episode != null)
            {
                _context.Episodes.Remove(episode);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Episode deleted successfully!";
            }
            return RedirectToAction(nameof(Episodes));
        }

        private bool EpisodeExists(int id)
        {
            return _context.Episodes.Any(e => e.EpisodeId == id);
        }

        // GET: Admin/Animes
        public async Task<IActionResult> Animes()
        {
            var animes = await _context.Animes
                .Include(a => a.AnimeGenres)
                .ThenInclude(ag => ag.Genre)
                .OrderBy(a => a.Title)
                .ToListAsync();
            return View(animes);
        }

        // GET: Admin/Animes/Create
        public IActionResult CreateAnime()
        {
            ViewData["Genres"] = new MultiSelectList(_context.Genres.OrderBy(g => g.Name), "GenreId", "Name");
            return View();
        }

        // POST: Admin/Animes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAnime([Bind("Title,Slug,Synopsis,Year,Rating,Status,EpisodeCount,Studio,CoverUrl,PosterUrl,TrailerUrl")] Anime anime, int[] selectedGenres)
        {
            if (ModelState.IsValid)
            {
                // Generate slug if not provided
                if (string.IsNullOrEmpty(anime.Slug))
                {
                    anime.Slug = anime.Title.ToLower().Replace(" ", "-").Replace(":", "").Replace("'", "");
                }

                _context.Add(anime);
                await _context.SaveChangesAsync();

                // Add genres
                if (selectedGenres != null && selectedGenres.Length > 0)
                {
                    foreach (var genreId in selectedGenres)
                    {
                        _context.AnimeGenres.Add(new AnimeGenre { AnimeId = anime.AnimeId, GenreId = genreId });
                    }
                    await _context.SaveChangesAsync();
                }

                TempData["Success"] = "Anime created successfully!";
                return RedirectToAction(nameof(Animes));
            }
            ViewData["Genres"] = new MultiSelectList(_context.Genres.OrderBy(g => g.Name), "GenreId", "Name", selectedGenres);
            return View(anime);
        }

        // GET: Admin/Animes/Edit/5
        public async Task<IActionResult> EditAnime(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var anime = await _context.Animes
                .Include(a => a.AnimeGenres)
                .FirstOrDefaultAsync(a => a.AnimeId == id);
            if (anime == null)
            {
                return NotFound();
            }

            var selectedGenres = anime.AnimeGenres.Select(ag => ag.GenreId).ToArray();
            ViewData["Genres"] = new MultiSelectList(_context.Genres.OrderBy(g => g.Name), "GenreId", "Name", selectedGenres);
            return View(anime);
        }

        // POST: Admin/Animes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAnime(int id, [Bind("AnimeId,Title,Slug,Synopsis,Year,Rating,Status,EpisodeCount,Studio,CoverUrl,PosterUrl,TrailerUrl")] Anime anime, int[] selectedGenres)
        {
            if (id != anime.AnimeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Generate slug if not provided
                    if (string.IsNullOrEmpty(anime.Slug))
                    {
                        anime.Slug = anime.Title.ToLower().Replace(" ", "-").Replace(":", "").Replace("'", "");
                    }

                    _context.Update(anime);

                    // Update genres
                    var existingGenres = _context.AnimeGenres.Where(ag => ag.AnimeId == id);
                    _context.AnimeGenres.RemoveRange(existingGenres);

                    if (selectedGenres != null && selectedGenres.Length > 0)
                    {
                        foreach (var genreId in selectedGenres)
                        {
                            _context.AnimeGenres.Add(new AnimeGenre { AnimeId = anime.AnimeId, GenreId = genreId });
                        }
                    }

                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Anime updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AnimeExists(anime.AnimeId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Animes));
            }
            ViewData["Genres"] = new MultiSelectList(_context.Genres.OrderBy(g => g.Name), "GenreId", "Name", selectedGenres);
            return View(anime);
        }

        // GET: Admin/Animes/Delete/5
        public async Task<IActionResult> DeleteAnime(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var anime = await _context.Animes
                .Include(a => a.AnimeGenres)
                .ThenInclude(ag => ag.Genre)
                .FirstOrDefaultAsync(m => m.AnimeId == id);
            if (anime == null)
            {
                return NotFound();
            }

            return View(anime);
        }

        // POST: Admin/Animes/Delete/5
        [HttpPost, ActionName("DeleteAnime")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAnimeConfirmed(int id)
        {
            var anime = await _context.Animes.FindAsync(id);
            if (anime != null)
            {
                // Remove related data first
                var animeGenres = _context.AnimeGenres.Where(ag => ag.AnimeId == id);
                _context.AnimeGenres.RemoveRange(animeGenres);

                var episodes = _context.Episodes.Where(e => e.AnimeId == id);
                _context.Episodes.RemoveRange(episodes);

                _context.Animes.Remove(anime);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Anime deleted successfully!";
            }
            return RedirectToAction(nameof(Animes));
        }

        private bool AnimeExists(int id)
        {
            return _context.Animes.Any(e => e.AnimeId == id);
        }

        // GET: Admin/Users
        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users.ToListAsync();
            var userViewModels = new List<dynamic>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userViewModels.Add(new
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Roles = roles
                });
            }

            return View(userViewModels);
        }

        // GET: Admin/News
        public async Task<IActionResult> News()
        {
            var news = await _context.News.OrderByDescending(n => n.PublishedAt).ToListAsync();
            return View(news);
        }

        // GET: Admin/News/Create
        public IActionResult CreateNews()
        {
            return View();
        }

        // POST: Admin/News/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateNews([Bind("Title,Content,Author,ImageUrl,NewsType,LinkUrl")] News news)
        {
            if (ModelState.IsValid)
            {
                news.PublishedAt = DateTime.UtcNow;
                news.Author ??= "Admin";
                _context.Add(news);
                await _context.SaveChangesAsync();
                TempData["Success"] = "News article created successfully!";
                return RedirectToAction(nameof(News));
            }
            return View(news);
        }

        // GET: Admin/News/Edit/5
        public async Task<IActionResult> EditNews(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var news = await _context.News.FindAsync(id);
            if (news == null)
            {
                return NotFound();
            }
            return View(news);
        }

        // POST: Admin/News/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditNews(int id, [Bind("NewsId,Title,Content,Author,ImageUrl,NewsType,LinkUrl,PublishedAt")] News news)
        {
            if (id != news.NewsId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(news);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "News article updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NewsExists(news.NewsId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(News));
            }
            return View(news);
        }

        // GET: Admin/News/Delete/5
        public async Task<IActionResult> DeleteNews(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var news = await _context.News.FirstOrDefaultAsync(m => m.NewsId == id);
            if (news == null)
            {
                return NotFound();
            }

            return View(news);
        }

        // POST: Admin/News/Delete/5
        [HttpPost, ActionName("DeleteNews")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteNewsConfirmed(int id)
        {
            var news = await _context.News.FindAsync(id);
            if (news != null)
            {
                // Delete related forum posts (handle self-referencing constraint)
                var forumPosts = await _context.ForumPosts
                    .Where(fp => fp.NewsId == id)
                    .ToListAsync();

                // Delete posts in reverse order (replies first)
                var postsToDelete = forumPosts.OrderByDescending(fp => fp.CreatedAt).ToList();
                foreach (var post in postsToDelete)
                {
                    // Set ParentPostId to null for any replies to avoid constraint issues
                    var replies = _context.ForumPosts.Where(fp => fp.ParentPostId == post.ForumPostId);
                    await replies.ForEachAsync(r => r.ParentPostId = null);
                    await _context.SaveChangesAsync();

                    _context.ForumPosts.Remove(post);
                }

                _context.News.Remove(news);
                await _context.SaveChangesAsync();
                TempData["Success"] = "News article deleted successfully!";
            }
            return RedirectToAction(nameof(News));
        }

        private bool NewsExists(int id)
        {
            return _context.News.Any(e => e.NewsId == id);
        }

        [HttpPost]
        public async Task<IActionResult> PromoteToAdmin(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return Json(new { success = false, message = "User not found" });
            }

            var result = await _userManager.AddToRoleAsync(user, "Admin");
            if (result.Succeeded)
            {
                TempData["Success"] = $"User {user.UserName} has been promoted to Admin";
                return Json(new { success = true });
            }

            return Json(new { success = false, message = string.Join(", ", result.Errors.Select(e => e.Description)) });
        }

        [HttpPost]
        public async Task<IActionResult> DemoteFromAdmin(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return Json(new { success = false, message = "User not found" });
            }

            // Prevent demoting the main admin account
            if (user.Email == "admin@animehub.com")
            {
                return Json(new { success = false, message = "Cannot demote the main admin account" });
            }

            var result = await _userManager.RemoveFromRoleAsync(user, "Admin");
            if (result.Succeeded)
            {
                TempData["Success"] = $"User {user.UserName} has been demoted from Admin";
                return Json(new { success = true });
            }

            return Json(new { success = false, message = string.Join(", ", result.Errors.Select(e => e.Description)) });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return Json(new { success = false, message = "User not found" });
            }

            // Prevent deleting the main admin account
            if (user.Email == "admin@animehub.com")
            {
                return Json(new { success = false, message = "Cannot delete the main admin account" });
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["Success"] = $"User {user.UserName} has been deleted";
                return Json(new { success = true });
            }

            return Json(new { success = false, message = string.Join(", ", result.Errors.Select(e => e.Description)) });
        }
    }
}