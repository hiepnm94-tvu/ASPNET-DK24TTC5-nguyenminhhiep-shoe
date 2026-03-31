using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using quanlybangiay.Data;
using quanlybangiay.Models;
using System.Security.Cryptography;
using System.Text;

namespace quanlybangiay.Controllers.Admin
{
    [Area("Admin")]
    [Authorize]
    public class CustomersController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CustomersController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 20)
        {
            var query = _db.Users
                .Include(u => u.Role)
                .OrderByDescending(u => u.CreatedAt)
                .AsQueryable();
            var items = await PaginatedList<User>.CreateAsync(query, page, pageSize);
            ViewBag.Page = items.PageIndex;
            ViewBag.PageSize = items.PageSize;
            ViewBag.TotalCount = items.TotalCount;
            ViewBag.TotalPages = items.TotalPages;
            return View(items);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var user = await _db.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == id.Value);
            if (user == null) return NotFound();
            return View(user);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Roles = new SelectList(
                await _db.Roles.OrderBy(r => r.RoleName).ToListAsync(),
                "RoleId", "RoleName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RoleId,FullName,Email,Phone,Gender,DateOfBirth,AvatarUrl,IsActive")] User user, string? Password)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrWhiteSpace(Password))
                {
                    user.PasswordHash = ComputeMd5(Password);
                }
                _db.Add(user);
                await _db.SaveChangesAsync();
                TempData["SuccessMessage"] = "Tạo khách hàng thành công!";
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = "Dữ liệu không hợp lệ, vui lòng kiểm tra lại.";
            ViewBag.Roles = new SelectList(
                await _db.Roles.OrderBy(r => r.RoleName).ToListAsync(),
                "RoleId", "RoleName", user.RoleId);
            return View(user);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var user = await _db.Users.FindAsync(id.Value);
            if (user == null) return NotFound();

            ViewBag.Roles = new SelectList(
                await _db.Roles.OrderBy(r => r.RoleName).ToListAsync(),
                "RoleId", "RoleName", user.RoleId);
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,RoleId,FullName,Email,Phone,Gender,DateOfBirth,AvatarUrl,IsActive")] User user, string? Password)
        {
            if (id != user.UserId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existing = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == id);
                    if (existing == null) return NotFound();

                    if (!string.IsNullOrWhiteSpace(Password))
                        user.PasswordHash = ComputeMd5(Password);
                    else
                        user.PasswordHash = existing.PasswordHash;

                    user.CreatedAt = existing.CreatedAt;
                    user.UpdatedAt = DateTime.UtcNow;
                    _db.Update(user);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _db.Users.AnyAsync(e => e.UserId == id))
                        return NotFound();
                    throw;
                }
                TempData["SuccessMessage"] = "Cập nhật khách hàng thành công!";
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = "Dữ liệu không hợp lệ, vui lòng kiểm tra lại.";
            ViewBag.Roles = new SelectList(
                await _db.Roles.OrderBy(r => r.RoleName).ToListAsync(),
                "RoleId", "RoleName", user.RoleId);
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user != null)
            {
                _db.Users.Remove(user);
                await _db.SaveChangesAsync();
                TempData["SuccessMessage"] = "Xóa khách hàng thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không tìm thấy khách hàng cần xóa.";
            }
            return RedirectToAction(nameof(Index));
        }

        private static string ComputeMd5(string input)
        {
            using var md5 = MD5.Create();
            var bytes = Encoding.UTF8.GetBytes(input ?? string.Empty);
            var hash = md5.ComputeHash(bytes);
            var sb = new StringBuilder();
            foreach (var b in hash)
                sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
    }
}
