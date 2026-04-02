using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quanlybangiay.Data;
using quanlybangiay.Helpers;
using quanlybangiay.Models;

namespace quanlybangiay.Controllers.Admin
{
    [Area("Admin")]
    [Authorize]
    public class SettingsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _env;

        public SettingsController(ApplicationDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        private int? GetCurrentUserId()
        {
            var val = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(val, out var id) ? id : null;
        }

        public async Task<IActionResult> Index()
        {
            var setting = await _db.Settings.FirstOrDefaultAsync();
            if (setting == null)
            {
                setting = new Setting { ShopName = "Shop Giày", CreatedBy = GetCurrentUserId() };
                _db.Settings.Add(setting);
                await _db.SaveChangesAsync();
            }
            return View(setting);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(
            [Bind("SettingId,ShopName,LogoUrl,BannerUrl,Phone,Hotline,Email,Address,WorkingHours," +
                  "FooterContent,CopyrightText,FacebookUrl,InstagramUrl,YoutubeUrl," +
                  "TiktokUrl,ZaloUrl,MetaTitle,MetaDescription,MetaKeywords,FaviconUrl")]
            Setting setting,
            IFormFile? logoFile,
            IFormFile? bannerFile)
        {
            var existing = await _db.Settings.FirstOrDefaultAsync();
            if (existing == null) return NotFound();

            if (setting.SettingId != existing.SettingId)
                setting.SettingId = existing.SettingId;

            if (ModelState.IsValid)
            {
                try
                {
                    // Handle logo upload
                    var newLogo = await FileUploadHelper.SaveAsync(logoFile, _env, "settings");
                    if (newLogo != null)
                    {
                        FileUploadHelper.Delete(_env, existing.LogoUrl);
                        existing.LogoUrl = newLogo;
                    }
                    else
                    {
                        existing.LogoUrl = setting.LogoUrl;
                    }

                    // Handle banner upload
                    var newBanner = await FileUploadHelper.SaveAsync(bannerFile, _env, "settings");
                    if (newBanner != null)
                    {
                        FileUploadHelper.Delete(_env, existing.BannerUrl);
                        existing.BannerUrl = newBanner;
                    }
                    else
                    {
                        existing.BannerUrl = setting.BannerUrl;
                    }
                }
                catch (InvalidOperationException ex)
                {
                    TempData["ErrorMessage"] = ex.Message;
                    return View(existing);
                }

                existing.ShopName = setting.ShopName;
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
                existing.UpdatedAt = DateTime.UtcNow;
                existing.UpdatedBy = GetCurrentUserId();

                await _db.SaveChangesAsync();
                TempData["SuccessMessage"] = "Lưu cài đặt thành công!";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "Dữ liệu không hợp lệ, vui lòng kiểm tra lại.";
            return View(setting);
        }
    }
}