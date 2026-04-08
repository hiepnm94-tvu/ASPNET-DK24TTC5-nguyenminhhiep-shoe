using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quanlybangiay.Data;
using quanlybangiay.Helpers;
using quanlybangiay.Models;
using quanlybangiay.Models.ViewModels;

namespace quanlybangiay.Controllers
{
    public class CartController : Controller
    {
        private const string CartSessionKey = "ShoppingCart";
        private readonly ApplicationDbContext _db;

        public CartController(ApplicationDbContext db)
        {
            _db = db;
        }

        private List<CartItem> GetCart()
        {
            return HttpContext.Session.GetObject<List<CartItem>>(CartSessionKey) ?? new List<CartItem>();
        }

        private void SaveCart(List<CartItem> cart)
        {
            HttpContext.Session.SetObject(CartSessionKey, cart);
        }

        public IActionResult Index()
        {
            var cart = GetCart();

            var relatedProducts = _db.Products
                .Where(p => p.Status == 1)
                .OrderByDescending(p => p.CreatedAt)
                .Take(4)
                .ToList();

            ViewBag.RelatedProducts = relatedProducts;
            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> Add(int productId, int variantId, int quantity = 1)
        {
            var variant = await _db.ProductVariants
                .Include(v => v.Product)
                .FirstOrDefaultAsync(v => v.VariantId == variantId);

            if (variant?.Product == null)
                return NotFound();

            var cart = GetCart();
            var existingItem = cart.FirstOrDefault(c => c.VariantId == variantId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                var unitPrice = variant.Product.DiscountPrice ?? variant.Product.BasePrice;
                if (variant.AdditionalPrice.HasValue)
                    unitPrice += variant.AdditionalPrice.Value;

                cart.Add(new CartItem
                {
                    ProductId = variant.ProductId,
                    VariantId = variant.VariantId,
                    ProductName = variant.Product.ProductName,
                    ThumbnailUrl = variant.Product.ThumbnailUrl,
                    SizeValue = variant.SizeValue,
                    ColorName = variant.ColorName,
                    UnitPrice = unitPrice,
                    Quantity = quantity
                });
            }

            SaveCart(cart);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Update(int variantId, int quantity)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.VariantId == variantId);
            if (item != null)
            {
                if (quantity <= 0)
                    cart.Remove(item);
                else
                    item.Quantity = quantity;
            }
            SaveCart(cart);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Remove(int variantId)
        {
            var cart = GetCart();
            cart.RemoveAll(c => c.VariantId == variantId);
            SaveCart(cart);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult GetCartCount()
        {
            var cart = GetCart();
            return Json(new { count = cart.Sum(c => c.Quantity) });
        }

        [HttpGet]
        public IActionResult TrackOrder()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> TrackOrder(string orderCode)
        {
            if (string.IsNullOrWhiteSpace(orderCode))
            {
                ViewBag.Error = "Vui lòng nhập mã đơn hàng.";
                return View();
            }

            var order = await _db.Orders
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.OrderCode == orderCode.Trim());

            if (order == null)
            {
                ViewBag.Error = "Không tìm thấy đơn hàng với mã này. Vui lòng kiểm tra lại.";
                ViewBag.SearchCode = orderCode;
                return View();
            }

            var orderItems = await _db.OrderItems
                .Where(oi => oi.OrderId == order.OrderId)
                .ToListAsync();

            ViewBag.Order = order;
            ViewBag.OrderItems = orderItems;
            ViewBag.SearchCode = orderCode;

            return View();
        }
    }
}
