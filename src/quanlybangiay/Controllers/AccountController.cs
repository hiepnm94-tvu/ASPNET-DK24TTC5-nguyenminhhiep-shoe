using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using quanlybangiay.Data;
using System.Security.Cryptography;
using System.Text;

namespace quanlybangiay.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AccountController(ApplicationDbContext db)
        {
            _db = db;
        }
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError(string.Empty, "Tên người dùng hoặc mật khẩu không đúng");
                return View();
            }

            // compute MD5 hash of provided password and compare with stored PasswordHash
            var passwordHash = ComputeMd5(password);
            // username is Email column
            var lookup = username.Trim().ToLowerInvariant();

            var user = await _db.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.IsActive
                    && u.Email != null && u.Email.ToLower() == lookup
                    && u.PasswordHash != null && u.PasswordHash == passwordHash);

            // require role Admin (RoleId == 1)
            if (user != null && user.RoleId.HasValue && user.RoleId.Value == 1)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Name, user.FullName ?? user.Email),
                    new Claim(ClaimTypes.Role, user.Role?.RoleName ?? "Admin"),
                    new Claim("Email", user.Email)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "Manage", new { Area = "Admin" });
            }

            ModelState.AddModelError(string.Empty, "Tên người dùng hoặc mật khẩu không đúng");
            return View();
        }

        [HttpGet]
        public IActionResult Logout()
        {
            // If accessed by GET, redirect to login page (do not sign out on GET)
            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Logout")]
        public async Task<IActionResult> LogoutPost()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        public IActionResult AccessDenied()
        {
            return Forbid();
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
