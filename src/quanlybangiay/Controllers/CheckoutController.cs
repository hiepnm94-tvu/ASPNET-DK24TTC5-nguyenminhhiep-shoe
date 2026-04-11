using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quanlybangiay.Data;
using quanlybangiay.Helpers;
using quanlybangiay.Models;
using quanlybangiay.Models.ViewModels;
using System.Security.Claims;

namespace quanlybangiay.Controllers
{
    public class CheckoutController : Controller
    {
        private const string CartSessionKey = "ShoppingCart";
        private readonly ApplicationDbContext _db;

        public CheckoutController(ApplicationDbContext db)
        {
            _db = db;
        }

        private List<CartItem> GetCart()
        {
            return HttpContext.Session.GetObject<List<CartItem>>(CartSessionKey) ?? new List<CartItem>();
        }

        public IActionResult Index()
        {
            var cart = GetCart();
            if (!cart.Any())
                return Redirect("/gio-hang");

            ViewBag.Cart = cart;
            ViewBag.Subtotal = cart.Sum(c => c.LineTotal);
            return View(new CheckoutViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(CheckoutViewModel model)
        {
            var cart = GetCart();
            if (!cart.Any())
                return Redirect("/gio-hang");

            if (!ModelState.IsValid)
            {
                ViewBag.Cart = cart;
                ViewBag.Subtotal = cart.Sum(c => c.LineTotal);
                return View("Index", model);
            }

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int? userId = int.TryParse(userIdStr, out var uid) ? uid : null;

            var subtotal = cart.Sum(c => c.LineTotal);
            var addressParts = new[] { model.Address, model.District, model.City }
                .Where(s => !string.IsNullOrWhiteSpace(s));

            var order = new Order
            {
                OrderCode = "ORD" + DateTime.UtcNow.ToString("yyyyMMddHHmmss") + new Random().Next(100, 999),
                UserId = userId,
                AddressId = null,
                GuestName = $"{model.FirstName} {model.LastName}".Trim(),
                GuestEmail = model.Email,
                GuestPhone = model.Phone,
                GuestAddress = string.Join(", ", addressParts),
                Subtotal = subtotal,
                ShippingFee = 0,
                DiscountAmount = 0,
                TotalAmount = subtotal,
                PaymentMethod = model.PaymentMethod,
                PaymentStatus = 0,
                OrderStatus = 0,
                Note = model.Note,
                CreatedAt = DateTime.UtcNow
            };

            _db.Orders.Add(order);
            await _db.SaveChangesAsync();

            foreach (var item in cart)
            {
                _db.OrderItems.Add(new OrderItem
                {
                    OrderId = order.OrderId,
                    VariantId = item.VariantId,
                    ProductName = item.ProductName,
                    SizeValue = item.SizeValue,
                    ColorName = item.ColorName,
                    UnitPrice = item.UnitPrice,
                    Quantity = item.Quantity,
                    LineTotal = item.LineTotal
                });
            }
            await _db.SaveChangesAsync();

            HttpContext.Session.Remove(CartSessionKey);

            return RedirectToRoute("checkout-complete", new { id = order.OrderId });
        }

        public async Task<IActionResult> Complete(long id)
        {
            var order = await _db.Orders.FindAsync(id);
            ViewBag.Order = order;
            return View();
        }
    }
}
