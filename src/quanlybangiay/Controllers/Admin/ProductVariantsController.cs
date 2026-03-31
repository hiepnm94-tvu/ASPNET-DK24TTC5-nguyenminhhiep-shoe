using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using quanlybangiay.Data;
using quanlybangiay.Models;

namespace quanlybangiay.Controllers.Admin
{
    [Area("Admin")]
    [Authorize]
    public class ProductVariantsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ProductVariantsController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(int? productId, int page = 1, int pageSize = 20)
        {
            var query = _db.ProductVariants.Include(v => v.Product).AsQueryable();

            if (productId.HasValue)
            {
                query = query.Where(v => v.ProductId == productId.Value);
                ViewBag.FilterProductId = productId.Value;
                ViewBag.FilterProductName = (await _db.Products.FindAsync(productId.Value))?.ProductName;
            }

            query = query.OrderBy(v => v.ProductId).ThenBy(v => v.SizeValue);
            var items = await PaginatedList<ProductVariant>.CreateAsync(query, page, pageSize);
            ViewBag.Page = items.PageIndex;
            ViewBag.PageSize = items.PageSize;
            ViewBag.TotalCount = items.TotalCount;
            ViewBag.TotalPages = items.TotalPages;
            return View(items);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var variant = await _db.ProductVariants
                .Include(v => v.Product)
                .FirstOrDefaultAsync(v => v.VariantId == id.Value);
            if (variant == null) return NotFound();
            return View(variant);
        }

        public async Task<IActionResult> Create(int? productId)
        {
            ViewBag.Products = new SelectList(
                await _db.Products.OrderBy(p => p.ProductName).ToListAsync(),
                "ProductId", "ProductName", productId);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,SKU,SizeValue,ColorName,ColorCode,StockQty,AdditionalPrice,WeightGram,IsDefault")] ProductVariant variant)
        {
            if (ModelState.IsValid)
            {
                _db.Add(variant);
                await _db.SaveChangesAsync();
                TempData["SuccessMessage"] = "Tạo biến thể sản phẩm thành công!";
                return RedirectToAction(nameof(Index), new { productId = variant.ProductId });
            }
            TempData["ErrorMessage"] = "Dữ liệu không hợp lệ, vui lòng kiểm tra lại.";
            ViewBag.Products = new SelectList(
                await _db.Products.OrderBy(p => p.ProductName).ToListAsync(),
                "ProductId", "ProductName", variant.ProductId);
            return View(variant);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var variant = await _db.ProductVariants.FindAsync(id.Value);
            if (variant == null) return NotFound();

            ViewBag.Products = new SelectList(
                await _db.Products.OrderBy(p => p.ProductName).ToListAsync(),
                "ProductId", "ProductName", variant.ProductId);
            return View(variant);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VariantId,ProductId,SKU,SizeValue,ColorName,ColorCode,StockQty,AdditionalPrice,WeightGram,IsDefault")] ProductVariant variant)
        {
            if (id != variant.VariantId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _db.Update(variant);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _db.ProductVariants.AnyAsync(e => e.VariantId == id))
                        return NotFound();
                    throw;
                }
                TempData["SuccessMessage"] = "Cập nhật biến thể sản phẩm thành công!";
                return RedirectToAction(nameof(Index), new { productId = variant.ProductId });
            }
            TempData["ErrorMessage"] = "Dữ liệu không hợp lệ, vui lòng kiểm tra lại.";
            ViewBag.Products = new SelectList(
                await _db.Products.OrderBy(p => p.ProductName).ToListAsync(),
                "ProductId", "ProductName", variant.ProductId);
            return View(variant);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int? productId)
        {
            var variant = await _db.ProductVariants.FindAsync(id);
            if (variant != null)
            {
                var pid = variant.ProductId;
                _db.ProductVariants.Remove(variant);
                await _db.SaveChangesAsync();
                TempData["SuccessMessage"] = "Xóa biến thể sản phẩm thành công!";
                return RedirectToAction(nameof(Index), new { productId = pid });
            }
            TempData["ErrorMessage"] = "Không tìm thấy biến thể cần xóa.";
            return RedirectToAction(nameof(Index), new { productId });
        }
    }
}
