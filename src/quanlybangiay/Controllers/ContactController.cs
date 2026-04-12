using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quanlybangiay.Data;
using quanlybangiay.Models;

namespace quanlybangiay.Controllers
{
    public class ContactController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ContactController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var setting = await _db.Settings.FirstOrDefaultAsync();
            ViewBag.Setting = setting;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(string firstName, string lastName, string email, string subject, string message)
        {
            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(message))
            {
                TempData["ErrorMessage"] = "Vui lòng điền đầy đủ thông tin bắt buộc.";
                return Redirect("/lien-he");
            }

            var contact = new Contact
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Subject = subject,
                Message = message,
                IP = HttpContext.Connection.RemoteIpAddress?.ToString(),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _db.Contacts.Add(contact);
            await _db.SaveChangesAsync();

            TempData["SuccessMessage"] = "Cảm ơn bạn đã liên hệ! Chúng tôi sẽ phản hồi sớm nhất.";
            return Redirect("/lien-he");
        }
    }
}
