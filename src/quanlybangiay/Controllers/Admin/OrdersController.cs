using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quanlybangiay.Data;
using quanlybangiay.Models;

namespace quanlybangiay.Controllers.Admin
{
    [Area("Admin")]
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _db;

        public OrdersController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 20)
        {
            var query = _db.Orders
                .Include(o => o.User)
                .OrderByDescending(o => o.CreatedAt)
                .AsQueryable();
            var items = await PaginatedList<Order>.CreateAsync(query, page, pageSize);
            ViewBag.Page = items.PageIndex;
            ViewBag.PageSize = items.PageSize;
            ViewBag.TotalCount = items.TotalCount;
            ViewBag.TotalPages = items.TotalPages;
            return View(items);
        }

        public async Task<IActionResult> Details(long? id)
        {
            if (id == null) return NotFound();
            var order = await _db.Orders
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.OrderId == id.Value);
            if (order == null) return NotFound();

            ViewBag.OrderItems = await _db.OrderItems
                .Include(oi => oi.Variant)
                .Where(oi => oi.OrderId == id.Value)
                .ToListAsync();

            ViewBag.Payments = await _db.Payments
                .Where(p => p.OrderId == id.Value)
                .OrderByDescending(p => p.PaidAt)
                .ToListAsync();

            return View(order);
        }

        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null) return NotFound();
            var order = await _db.Orders
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.OrderId == id.Value);
            if (order == null) return NotFound();
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, byte? OrderStatus, byte? PaymentStatus, string Note)
        {
            var order = await _db.Orders.FindAsync(id);
            if (order == null) return NotFound();

            order.OrderStatus = OrderStatus;
            order.PaymentStatus = PaymentStatus;
            order.Note = Note;
            order.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = "Cập nhật đơn hàng thành công!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(long id)
        {
            var order = await _db.Orders.FindAsync(id);
            if (order != null)
            {
                _db.Orders.Remove(order);
                await _db.SaveChangesAsync();
                TempData["SuccessMessage"] = "Xóa đơn hàng thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không tìm thấy đơn hàng cần xóa.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
