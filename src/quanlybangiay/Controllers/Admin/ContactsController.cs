using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quanlybangiay.Data;
using quanlybangiay.Models;

namespace quanlybangiay.Controllers.Admin
{
    [Area("Admin")]
    [Authorize]
    public class ContactsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ContactsController(ApplicationDbContext db)
        {
            _db = db;
        }

        private int? GetCurrentUserId()
        {
            var val = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(val, out var id) ? id : null;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 20)
        {
            var query = _db.Contacts.OrderByDescending(c => c.CreatedAt).AsQueryable();
            var items = await PaginatedList<Contact>.CreateAsync(query, page, pageSize);
            ViewBag.Page = items.PageIndex;
            ViewBag.PageSize = items.PageSize;
            ViewBag.TotalCount = items.TotalCount;
            ViewBag.TotalPages = items.TotalPages;
            return View(items);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var contact = await _db.Contacts.FindAsync(id.Value);
            if (contact == null) return NotFound();
            return View(contact);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var contact = await _db.Contacts.FindAsync(id.Value);
            if (contact == null) return NotFound();
            return View(contact);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ContactId,IsActive,Note")] Contact input)
        {
            var contact = await _db.Contacts.FindAsync(id);
            if (contact == null) return NotFound();

            contact.IsActive = input.IsActive;
            contact.Note = input.Note;
            contact.UpdatedBy = GetCurrentUserId();

            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = "Cập nhật liên hệ thành công!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var contact = await _db.Contacts.FindAsync(id);
            if (contact != null)
            {
                _db.Contacts.Remove(contact);
                await _db.SaveChangesAsync();
                TempData["SuccessMessage"] = "Xóa liên hệ thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không tìm thấy liên hệ cần xóa.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
