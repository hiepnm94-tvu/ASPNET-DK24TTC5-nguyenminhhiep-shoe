using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quanlybangiay.Data;

namespace quanlybangiay.Controllers
{
    public class PostController : Controller
    {
        private readonly ApplicationDbContext _db;

        public PostController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 12)
        {
            var query = _db.Posts
                .Where(p => p.IsActive == true)
                .OrderByDescending(p => p.CreatedAt)
                .AsQueryable();

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            page = Math.Max(1, Math.Min(page, Math.Max(1, totalPages)));

            var posts = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalCount = totalCount;

            return View(posts);
        }

        public async Task<IActionResult> Detail(string? slug, int? id)
        {
            Models.Post? post = null;

            if (!string.IsNullOrWhiteSpace(slug))
            {
                post = await _db.Posts
                    .Include(p => p.CreatedByUser)
                    .FirstOrDefaultAsync(p => p.Slug == slug && p.IsActive == true);
            }
            else if (id.HasValue)
            {
                post = await _db.Posts
                    .Include(p => p.CreatedByUser)
                    .FirstOrDefaultAsync(p => p.PostId == id.Value && p.IsActive == true);

                if (post != null && !string.IsNullOrWhiteSpace(post.Slug))
                    return RedirectToRoutePermanent("post-detail", new { slug = post.Slug });
            }

            if (post == null)
                return NotFound();

            var recentPosts = await _db.Posts
                .Where(p => p.IsActive == true && p.PostId != post.PostId)
                .OrderByDescending(p => p.CreatedAt)
                .Take(5)
                .ToListAsync();

            ViewBag.RecentPosts = recentPosts;

            return View(post);
        }
    }
}
