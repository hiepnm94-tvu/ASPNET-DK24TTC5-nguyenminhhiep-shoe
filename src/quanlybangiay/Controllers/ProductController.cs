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

        public async Task<IActionResult> Category(string? slug, int? id, int page = 1, int pageSize = 12)
        {
            var categoriesQuery = _db.Categories.Where(c => c.IsActive == true);
            var categories = await categoriesQuery.OrderBy(c => c.CategoryName).ToListAsync();

            var query = _db.Products
                .Include(p => p.Category)
                .Where(p => p.Status == 1)
                .AsQueryable();

            Models.Category? currentCategory = null;

            if (!string.IsNullOrWhiteSpace(slug))
            {
                currentCategory = await _db.Categories.FirstOrDefaultAsync(c => c.Slug == slug && c.IsActive == true);
                if (currentCategory != null)
                    query = query.Where(p => p.CategoryId == currentCategory.CategoryId);
            }
            else if (id.HasValue && id > 0)
            {
                query = query.Where(p => p.CategoryId == id.Value);
                currentCategory = await _db.Categories.FindAsync(id.Value);
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            page = Math.Max(1, Math.Min(page, Math.Max(1, totalPages)));

            var products = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Categories = categories;
            ViewBag.CurrentCategory = currentCategory;
            ViewBag.CategoryId = currentCategory?.CategoryId ?? id;
            ViewBag.CategorySlug = currentCategory?.Slug;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalCount = totalCount;

            return View(products);
        }

        public async Task<IActionResult> Detail(string? slug, int? id)
        {
            Models.Product? product = null;

            if (!string.IsNullOrWhiteSpace(slug))
            {
                product = await _db.Products
                    .Include(p => p.Category)
                    .FirstOrDefaultAsync(p => p.Slug == slug && p.Status == 1);
            }
            else if (id.HasValue)
            {
                product = await _db.Products
                    .Include(p => p.Category)
                    .FirstOrDefaultAsync(p => p.ProductId == id.Value && p.Status == 1);

                if (product != null && !string.IsNullOrWhiteSpace(product.Slug))
                    return RedirectToRoutePermanent("product-detail", new { slug = product.Slug });
            }

            if (product == null)
                return NotFound();

            var variants = await _db.ProductVariants
                .Where(v => v.ProductId == product.ProductId && v.StockQty > 0)
                .OrderBy(v => v.SizeValue)
                .ToListAsync();

            var relatedProducts = await _db.Products
                .Where(p => p.CategoryId == product.CategoryId && p.ProductId != product.ProductId && p.Status == 1)
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
