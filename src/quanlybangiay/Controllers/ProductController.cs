using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quanlybangiay.Data;

namespace quanlybangiay.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ProductController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Category(int? id, int page = 1, int pageSize = 12)
        {
            var categoriesQuery = _db.Categories.Where(c => c.IsActive == true);
            var categories = await categoriesQuery.OrderBy(c => c.CategoryName).ToListAsync();

            var query = _db.Products
                .Include(p => p.Category)
                .Where(p => p.Status == 1)
                .AsQueryable();

            if (id.HasValue && id > 0)
            {
                query = query.Where(p => p.CategoryId == id.Value);
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            page = Math.Max(1, Math.Min(page, Math.Max(1, totalPages)));

            var products = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var currentCategory = id.HasValue
                ? await _db.Categories.FindAsync(id.Value)
                : null;

            ViewBag.Categories = categories;
            ViewBag.CurrentCategory = currentCategory;
            ViewBag.CategoryId = id;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalCount = totalCount;

            return View(products);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var product = await _db.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductId == id && p.Status == 1);

            if (product == null)
                return NotFound();

            var variants = await _db.ProductVariants
                .Where(v => v.ProductId == id && v.StockQty > 0)
                .OrderBy(v => v.SizeValue)
                .ToListAsync();

            var relatedProducts = await _db.Products
                .Where(p => p.CategoryId == product.CategoryId && p.ProductId != id && p.Status == 1)
                .Take(4)
                .ToListAsync();

            ViewBag.Variants = variants;
            ViewBag.RelatedProducts = relatedProducts;

            return View(product);
        }

        public async Task<IActionResult> Search(string? q, int page = 1, int pageSize = 12)
        {
            var query = _db.Products
                .Include(p => p.Category)
                .Where(p => p.Status == 1)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                query = query.Where(p =>
                    p.ProductName.Contains(q) ||
                    (p.Brand != null && p.Brand.Contains(q)) ||
                    (p.ShortDescription != null && p.ShortDescription.Contains(q)));
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            page = Math.Max(1, Math.Min(page, Math.Max(1, totalPages)));

            var products = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.SearchQuery = q;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalCount = totalCount;

            return View(products);
        }
    }
}
