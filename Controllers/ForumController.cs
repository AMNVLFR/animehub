using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AnimeHub.Data;
using AnimeHub.Models;

namespace AnimeHub.Controllers
{
    public class ForumController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ForumController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Forum/Discussion/5
        public async Task<IActionResult> Discussion(int id)
        {
            var news = await _context.News
                .Include(n => n.ForumPosts)
                .ThenInclude(fp => fp.User)
                .FirstOrDefaultAsync(n => n.NewsId == id);

            if (news == null)
            {
                return NotFound();
            }

            // Only allow forum access for internal news
            if (news.NewsType != NewsType.Internal)
            {
                return Redirect(news.LinkUrl ?? "/");
            }

            return View(news);
        }

        // POST: Forum/CreatePost
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost(int newsId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                TempData["Error"] = "Post content cannot be empty.";
                return RedirectToAction("Discussion", new { id = newsId });
            }

            var news = await _context.News.FindAsync(newsId);
            if (news == null || news.NewsType != NewsType.Internal)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var forumPost = new ForumPost
            {
                NewsId = newsId,
                UserId = user.Id,
                Content = content.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            _context.ForumPosts.Add(forumPost);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Your post has been added to the discussion.";
            return RedirectToAction("Discussion", new { id = newsId });
        }

        // POST: Forum/CreateReply
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateReply(int newsId, int parentPostId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                TempData["Error"] = "Reply content cannot be empty.";
                return RedirectToAction("Discussion", new { id = newsId });
            }

            var news = await _context.News.FindAsync(newsId);
            if (news == null || news.NewsType != NewsType.Internal)
            {
                return NotFound();
            }

            var parentPost = await _context.ForumPosts.FindAsync(parentPostId);
            if (parentPost == null || parentPost.NewsId != newsId)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var reply = new ForumPost
            {
                NewsId = newsId,
                UserId = user.Id,
                Content = content.Trim(),
                ParentPostId = parentPostId,
                CreatedAt = DateTime.UtcNow
            };

            _context.ForumPosts.Add(reply);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Your reply has been posted.";
            return RedirectToAction("Discussion", new { id = newsId });
        }
    }
}