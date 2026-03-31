using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quanlybangiay.Data;

namespace quanlybangiay.Controllers.Admin
{
    [Area("Admin")]
    [Authorize]
    public class ManageController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ManageController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(string? range)
        {
            DateTime fromDate;
            DateTime toDate = DateTime.UtcNow;
            string activeRange;

            switch (range)
            {
                case "today":
                    fromDate = DateTime.UtcNow.Date;
                    activeRange = "today";
                    break;
                case "7days":
                    fromDate = DateTime.UtcNow.Date.AddDays(-6);
                    activeRange = "7days";
                    break;
                case "year":
                    fromDate = new DateTime(DateTime.UtcNow.Year, 1, 1);
                    activeRange = "year";
                    break;
                case "all":
                    fromDate = DateTime.MinValue;
                    activeRange = "all";
                    break;
                default:
                    fromDate = DateTime.UtcNow.Date.AddDays(-29);
                    activeRange = "30days";
                    break;
            }

            ViewBag.ActiveRange = activeRange;

            var ordersInRange = _db.Orders.Where(o => o.CreatedAt >= fromDate && o.CreatedAt <= toDate);

            var totalRevenue = await ordersInRange
                .Where(o => o.OrderStatus != 4)
                .SumAsync(o => (decimal?)o.TotalAmount) ?? 0;

            var totalOrders = await ordersInRange.CountAsync();

            var paidOrders = await ordersInRange
                .Where(o => o.PaymentStatus == 1)
                .CountAsync();

            var pendingOrders = await ordersInRange
                .Where(o => o.OrderStatus == 0)
                .CountAsync();

            var cancelledOrders = await ordersInRange
                .Where(o => o.OrderStatus == 4)
                .CountAsync();

            ViewBag.TotalRevenue = totalRevenue;
            ViewBag.TotalOrders = totalOrders;
            ViewBag.PaidOrders = paidOrders;
            ViewBag.PendingOrders = pendingOrders;
            ViewBag.CancelledOrders = cancelledOrders;

            var totalProducts = await _db.Products.CountAsync();
            var totalCustomers = await _db.Users.CountAsync();
            ViewBag.TotalProducts = totalProducts;
            ViewBag.TotalCustomers = totalCustomers;

            var last30Days = Enumerable.Range(0, 30)
                .Select(i => DateTime.UtcNow.Date.AddDays(-29 + i))
                .ToList();

            var revenueByDay = await _db.Orders
                .Where(o => o.CreatedAt >= last30Days.First() && o.OrderStatus != 4)
                .GroupBy(o => o.CreatedAt.Date)
                .Select(g => new { Date = g.Key, Total = g.Sum(o => o.TotalAmount) })
                .ToDictionaryAsync(x => x.Date, x => x.Total);

            var chartLabels = last30Days.Select(d => d.ToString("dd/MM")).ToList();
            var chartData = last30Days.Select(d => revenueByDay.ContainsKey(d) ? revenueByDay[d] : 0).ToList();

            ViewBag.ChartLabels = chartLabels;
            ViewBag.ChartData = chartData;

            var topProducts = await _db.OrderItems
                .Where(oi => oi.Order != null && oi.Order.OrderStatus != 4)
                .GroupBy(oi => new { oi.ProductName })
                .Select(g => new
                {
                    ProductName = g.Key.ProductName,
                    TotalQty = g.Sum(x => x.Quantity),
                    TotalSales = g.Sum(x => x.LineTotal)
                })
                .OrderByDescending(x => x.TotalQty)
                .Take(10)
                .ToListAsync();

            ViewBag.TopProducts = topProducts.Select(p => new
            {
                p.ProductName,
                p.TotalQty,
                TotalSales = p.TotalSales.ToString("N0")
            }).ToList();

            const int lowStockThreshold = 5;
            var lowStockItems = await _db.ProductVariants
                .Include(v => v.Product)
                .Where(v => v.StockQty <= lowStockThreshold)
                .OrderBy(v => v.StockQty)
                .Take(20)
                .ToListAsync();

            ViewBag.LowStockItems = lowStockItems;
            ViewBag.LowStockThreshold = lowStockThreshold;

            return View();
        }
    }
}
