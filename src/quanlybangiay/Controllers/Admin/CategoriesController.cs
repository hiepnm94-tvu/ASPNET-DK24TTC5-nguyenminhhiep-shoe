using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quanlybangiay.Data;
using quanlybangiay.Models;
using System.Threading.Tasks;
using System.Linq;

namespace quanlybangiay.Controllers.Admin
{
    [Area("Admin")]
    [Authorize]
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CategoriesController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 20)
        {
            var query = _db.Categories.OrderBy(c => c.CategoryId).AsQueryable();
            var items = await PaginatedList<Category>.CreateAsync(query, page, pageSize);
            ViewBag.Page = items.PageIndex;
            ViewBag.PageSize = items.PageSize;
            ViewBag.TotalCount = items.TotalCount;
            ViewBag.TotalPages = items.TotalPages;
            return View(items);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var category = await _db.Categories.FindAsync(id.Value);
            if (category == null) return NotFound();
            return View(category);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryName,Slug,IsActive")] Category category)
        {
            if (ModelState.IsValid)
            {
                _db.Add(category);
                await _db.SaveChangesAsync();
                TempData["SuccessMessage"] = "Tạo chuyên mục thành công!";
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = "Dữ liệu không hợp lệ, vui lòng kiểm tra lại.";
            return View(category);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var category = await _db.Categories.FindAsync(id.Value);
            if (category == null) return NotFound();
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CategoryId,CategoryName,Slug,IsActive")] Category category)
        {
            if (id != category.CategoryId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _db.Update(category);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _db.Categories.AnyAsync(e => e.CategoryId == id))
                        return NotFound();
                    throw;
                }
                TempData["SuccessMessage"] = "Cập nhật chuyên mục thành công!";
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = "Dữ liệu không hợp lệ, vui lòng kiểm tra lại.";
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _db.Categories.FindAsync(id);
            if (category != null)
            {
                _db.Categories.Remove(category);
                await _db.SaveChangesAsync();
                TempData["SuccessMessage"] = "Xóa chuyên mục thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không tìm thấy chuyên mục cần xóa.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
