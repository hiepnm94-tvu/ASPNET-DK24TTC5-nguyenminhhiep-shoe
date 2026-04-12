using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quanlybangiay.Data;
using quanlybangiay.Helpers;
using quanlybangiay.Models;

namespace quanlybangiay.Controllers.Admin
{
    [Area("Admin")]
    [Authorize]
    public class SlidersController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _env;

        public SlidersController(ApplicationDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 20)
        {
            var query = _db.Sliders.OrderBy(s => s.SortOrder).ThenByDescending(s => s.CreatedAt).AsQueryable();
            var items = await PaginatedList<Slider>.CreateAsync(query, page, pageSize);
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
            var slider = await _db.Sliders
                .Include(s => s.CreatedByUser)
                .Include(s => s.UpdatedByUser)
                .FirstOrDefaultAsync(s => s.SliderId == id.Value);
            if (slider == null) return NotFound();
            return View(slider);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Title,Subtitle,LinkUrl,SortOrder,IsActive")] Slider slider,
            IFormFile? imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                ModelState.AddModelError("imageFile", "Vui lòng chọn hình ảnh cho slider.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    slider.Image = await FileUploadHelper.SaveAsync(imageFile, _env, "sliders") ?? string.Empty;
                    slider.CreatedBy = GetCurrentUserId();
                    _db.Add(slider);
                    await _db.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Tạo slider thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("imageFile", ex.Message);
                }
            }
            TempData["ErrorMessage"] = "Dữ liệu không hợp lệ, vui lòng kiểm tra lại.";
            return View(slider);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var slider = await _db.Sliders.FindAsync(id.Value);
            if (slider == null) return NotFound();
            return View(slider);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("SliderId,Title,Subtitle,LinkUrl,Image,SortOrder,IsActive")] Slider slider,
            IFormFile? imageFile)
        {
            if (id != slider.SliderId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existing = await _db.Sliders.AsNoTracking().FirstOrDefaultAsync(s => s.SliderId == id);
                    slider.CreatedBy = existing?.CreatedBy;
                    slider.CreatedAt = existing?.CreatedAt ?? DateTime.UtcNow;
                    slider.UpdatedBy = GetCurrentUserId();
                    slider.UpdatedAt = DateTime.UtcNow;

                    var newPath = await FileUploadHelper.SaveAsync(imageFile, _env, "sliders");
                    if (newPath != null)
                    {
                        FileUploadHelper.Delete(_env, slider.Image);
                        slider.Image = newPath;
                    }

                    _db.Update(slider);
                    await _db.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật slider thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("imageFile", ex.Message);
                    return View(slider);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _db.Sliders.AnyAsync(e => e.SliderId == id))
                        return NotFound();
                    throw;
                }
            }
            TempData["ErrorMessage"] = "Dữ liệu không hợp lệ, vui lòng kiểm tra lại.";
            return View(slider);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var slider = await _db.Sliders.FindAsync(id);
            if (slider != null)
            {
                FileUploadHelper.Delete(_env, slider.Image);
                _db.Sliders.Remove(slider);
                await _db.SaveChangesAsync();
                TempData["SuccessMessage"] = "Xóa slider thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không tìm thấy slider cần xóa.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
