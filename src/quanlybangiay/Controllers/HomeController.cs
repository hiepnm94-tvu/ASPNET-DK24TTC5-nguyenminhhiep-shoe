using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quanlybangiay.Data;
using quanlybangiay.Models;
using System.Diagnostics;

namespace quanlybangiay.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _db.Categories
                .Where(c => c.IsActive == true)
                .OrderBy(c => c.CategoryName)
                .ToListAsync();

            var bestSellers = await _db.Products
                .Include(p => p.Category)
                .Where(p => p.Status == 1)
                .OrderByDescending(p => p.CreatedAt)
                .Take(8)
                .ToListAsync();

            var sliders = await _db.Sliders
                .Where(s => s.IsActive)
                .OrderBy(s => s.SortOrder)
                .ThenByDescending(s => s.CreatedAt)
                .ToListAsync();

            ViewBag.Categories = categories;
            ViewBag.BestSellers = bestSellers;
            ViewBag.Sliders = sliders;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
