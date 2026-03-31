using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quanlybangiay.Data;
using quanlybangiay.Models;

namespace quanlybangiay.Controllers.Admin
{
    [Area("Admin")]
    [Authorize]
    public class SettingsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public SettingsController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var setting = await _db.Settings.FirstOrDefaultAsync();
            if (setting == null)
            {
                setting = new Setting { ShopName = "Shop Giày" };
                _db.Settings.Add(setting);
                await _db.SaveChangesAsync();
            }
            return View(setting);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(
            [Bind("SettingId,ShopName,LogoUrl,Phone,Hotline,Email,Address,WorkingHours," +
                  "FooterContent,CopyrightText,FacebookUrl,InstagramUrl,YoutubeUrl," +
                  "TiktokUrl,ZaloUrl,MetaTitle,MetaDescription,MetaKeywords,FaviconUrl,BannerUrl")]
            Setting setting)
        {
            var existing = await _db.Settings.FirstOrDefaultAsync();
            if (existing == null) return NotFound();

            if (setting.SettingId != existing.SettingId)
                setting.SettingId = existing.SettingId;

            if (ModelState.IsValid)
            {
                existing.ShopName = setting.ShopName;
                existing.LogoUrl = setting.LogoUrl;
                existing.Phone = setting.Phone;
                existing.Hotline = setting.Hotline;
                existing.Email = setting.Email;
                existing.Address = setting.Address;
                existing.WorkingHours = setting.WorkingHours;
                existing.FooterContent = setting.FooterContent;
                existing.CopyrightText = setting.CopyrightText;
                existing.FacebookUrl = setting.FacebookUrl;
                existing.InstagramUrl = setting.InstagramUrl;
                existing.YoutubeUrl = setting.YoutubeUrl;
                existing.TiktokUrl = setting.TiktokUrl;
                existing.ZaloUrl = setting.ZaloUrl;
                existing.MetaTitle = setting.MetaTitle;
                existing.MetaDescription = setting.MetaDescription;
                existing.MetaKeywords = setting.MetaKeywords;
                existing.FaviconUrl = setting.FaviconUrl;
                existing.BannerUrl = setting.BannerUrl;
                existing.UpdatedAt = DateTime.UtcNow;

                await _db.SaveChangesAsync();
                TempData["SuccessMessage"] = "Lưu cài đặt thành công!";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "Dữ liệu không hợp lệ, vui lòng kiểm tra lại.";
            return View(setting);
        }
    }
}
