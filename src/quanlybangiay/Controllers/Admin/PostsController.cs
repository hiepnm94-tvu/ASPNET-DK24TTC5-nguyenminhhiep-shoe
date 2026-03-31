using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quanlybangiay.Data;
using quanlybangiay.Models;

namespace quanlybangiay.Controllers.Admin
{
    [Area("Admin")]
    [Authorize]
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public PostsController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 20)
        {
            var query = _db.Posts.OrderByDescending(p => p.CreatedAt).AsQueryable();
            var items = await PaginatedList<Post>.CreateAsync(query, page, pageSize);
            ViewBag.Page = items.PageIndex;
            ViewBag.PageSize = items.PageSize;
            ViewBag.TotalCount = items.TotalCount;
            ViewBag.TotalPages = items.TotalPages;
            return View(items);
        }

        private int? GetCurrentUserId()
        {
            var val = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(val, out var id) ? id : null;
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var post = await _db.Posts
                .Include(p => p.CreatedByUser)
                .Include(p => p.UpdatedByUser)
                .FirstOrDefaultAsync(p => p.PostId == id.Value);
            if (post == null) return NotFound();
            return View(post);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Slug,ShortDescription,Content,ThumbnailUrl,IsActive")] Post post)
        {
            if (ModelState.IsValid)
            {
                post.CreatedBy = GetCurrentUserId();
                _db.Add(post);
                await _db.SaveChangesAsync();
                TempData["SuccessMessage"] = "Tạo bài viết thành công!";
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = "Dữ liệu không hợp lệ, vui lòng kiểm tra lại.";
            return View(post);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var post = await _db.Posts.FindAsync(id.Value);
            if (post == null) return NotFound();
            return View(post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PostId,Title,Slug,ShortDescription,Content,ThumbnailUrl,IsActive")] Post post)
        {
            if (id != post.PostId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existing = await _db.Posts.AsNoTracking().FirstOrDefaultAsync(p => p.PostId == id);
                    post.CreatedBy = existing?.CreatedBy;
                    post.UpdatedBy = GetCurrentUserId();
                    post.UpdatedAt = DateTime.UtcNow;
                    _db.Update(post);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _db.Posts.AnyAsync(e => e.PostId == id))
                        return NotFound();
                    throw;
                }
                TempData["SuccessMessage"] = "Cập nhật bài viết thành công!";
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = "Dữ liệu không hợp lệ, vui lòng kiểm tra lại.";
            return View(post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var post = await _db.Posts.FindAsync(id);
            if (post != null)
            {
                _db.Posts.Remove(post);
                await _db.SaveChangesAsync();
                TempData["SuccessMessage"] = "Xóa bài viết thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không tìm thấy bài viết cần xóa.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
