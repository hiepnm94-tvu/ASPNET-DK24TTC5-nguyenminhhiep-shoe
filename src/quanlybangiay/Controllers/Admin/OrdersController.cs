using System.Text;
using System.Text.Json;
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

        public async Task<IActionResult> Index(
            string? orderCode, string? customerName, byte? orderStatus,
            DateTime? dateFrom, DateTime? dateTo,
            int page = 1, int pageSize = 20)
        {
            var query = _db.Orders
                .Include(o => o.User)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(orderCode))
                query = query.Where(o => o.OrderCode.Contains(orderCode));

            if (!string.IsNullOrWhiteSpace(customerName))
            {
                var name = customerName.Trim();
                query = query.Where(o =>
                    (o.User != null && o.User.FullName != null && o.User.FullName.Contains(name)) ||
                    (o.GuestName != null && o.GuestName.Contains(name)));
            }

            if (orderStatus.HasValue)
                query = query.Where(o => o.OrderStatus == orderStatus.Value);

            if (dateFrom.HasValue)
                query = query.Where(o => o.CreatedAt >= dateFrom.Value);

            if (dateTo.HasValue)
            {
                var endDate = dateTo.Value.Date.AddDays(1);
                query = query.Where(o => o.CreatedAt < endDate);
            }

            query = query.OrderByDescending(o => o.CreatedAt);

            var items = await PaginatedList<Order>.CreateAsync(query, page, pageSize);

            ViewBag.Page = items.PageIndex;
            ViewBag.PageSize = items.PageSize;
            ViewBag.TotalCount = items.TotalCount;
            ViewBag.TotalPages = items.TotalPages;

            ViewBag.OrderCode = orderCode;
            ViewBag.CustomerName = customerName;
            ViewBag.OrderStatus = orderStatus;
            ViewBag.DateFrom = dateFrom;
            ViewBag.DateTo = dateTo;

            return View(items);
        }

        public async Task<IActionResult> Export(string period = "day", string format = "csv")
        {
            var now = DateTime.UtcNow;
            DateTime from = period switch
            {
                "week" => now.AddDays(-7),
                "month" => now.AddMonths(-1),
                _ => now.AddDays(-1)
            };

            var orders = await _db.Orders
                .Include(o => o.User)
                .Where(o => o.CreatedAt >= from)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            string fileName = $"orders-{period}-{now:yyyyMMdd_HHmmss}";

            if (format == "json")
                return ExportJson(orders, fileName);

            return ExportCsv(orders, fileName);
        }

        private FileContentResult ExportCsv(List<Order> orders, string fileName)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Mã đơn,Khách hàng,Email,SĐT,Tổng tiền,Trạng thái đơn,Thanh toán,Phương thức TT,Ngày đặt");

            foreach (var o in orders)
            {
                var customer = o.User?.FullName ?? o.GuestName ?? "Khách";
                var email = o.User?.Email ?? o.GuestEmail ?? "";
                var phone = o.User?.Phone ?? o.GuestPhone ?? "";
                var orderStatusText = OrderStatusText(o.OrderStatus);
                var paymentStatusText = PaymentStatusText(o.PaymentStatus);

                sb.AppendLine(string.Join(",",
                    Escape(o.OrderCode),
                    Escape(customer),
                    Escape(email),
                    Escape(phone),
                    o.TotalAmount.ToString("F0"),
                    Escape(orderStatusText),
                    Escape(paymentStatusText),
                    Escape(o.PaymentMethod ?? ""),
                    o.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")));
            }

            var bom = Encoding.UTF8.GetPreamble();
            var csvBytes = Encoding.UTF8.GetBytes(sb.ToString());
            var result = new byte[bom.Length + csvBytes.Length];
            bom.CopyTo(result, 0);
            csvBytes.CopyTo(result, bom.Length);

            return File(result, "text/csv; charset=utf-8", $"{fileName}.csv");
        }

        private FileContentResult ExportJson(List<Order> orders, string fileName)
        {
            var data = orders.Select(o => new
            {
                o.OrderCode,
                Customer = o.User?.FullName ?? o.GuestName ?? "Khách",
                Email = o.User?.Email ?? o.GuestEmail ?? "",
                Phone = o.User?.Phone ?? o.GuestPhone ?? "",
                o.TotalAmount,
                OrderStatus = OrderStatusText(o.OrderStatus),
                PaymentStatus = PaymentStatusText(o.PaymentStatus),
                o.PaymentMethod,
                CreatedAt = o.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
            });

            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            var bytes = Encoding.UTF8.GetBytes(json);
            return File(bytes, "application/json", $"{fileName}.json");
        }

        private static string Escape(string value)
        {
            if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
                return $"\"{value.Replace("\"", "\"\"")}\"";
            return value;
        }

        private static string OrderStatusText(byte? status) => status switch
        {
            0 => "Chờ xác nhận",
            1 => "Đã xác nhận",
            2 => "Đang giao",
            3 => "Đã giao",
            4 => "Đã hủy",
            _ => "Không rõ"
        };

        private static string PaymentStatusText(byte? status) => status switch
        {
            0 => "Chưa thanh toán",
            1 => "Đã thanh toán",
            2 => "Hoàn tiền",
            _ => "Không rõ"
        };

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
        public async Task<IActionResult> Edit(long id, byte? OrderStatus, byte? PaymentStatus, string? PaymentMethod, string? Note)
        {
            var order = await _db.Orders.FindAsync(id);
            if (order == null) return NotFound();

            order.OrderStatus = OrderStatus;
            order.PaymentStatus = PaymentStatus;
            order.PaymentMethod = PaymentMethod;
            order.Note = Note;
            order.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = "Cập nhật đơn hàng thành công!";
            return RedirectToAction(nameof(Details), new { id });
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
