using Microsoft.EntityFrameworkCore;
using quanlybangiay.Data;
using quanlybangiay.Config;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register ApplicationDbContext (centralized in config/DatabaseConfig.cs)
builder.Services.AddDatabase(builder.Configuration);

// Session for shopping cart
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add cookie authentication
builder.Services.AddAuthentication(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // Unhandled exceptions will be caught and redirected to /Error
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Handle specific status codes (e.g., 404) by redirecting to /Error/{statusCode}
app.UseStatusCodePagesWithReExecute("/Error/{0}");

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

// Admin area route
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Manage}/{action=Index}/{id?}");

// --- SEO-friendly storefront routes ---

// PDP – Product Detail Page by slug
app.MapControllerRoute(
    name: "product-detail",
    pattern: "san-pham/{slug}",
    defaults: new { controller = "Product", action = "Detail" });

// PLP – Product Listing / Category by slug
app.MapControllerRoute(
    name: "category-by-slug",
    pattern: "danh-muc/{slug}",
    defaults: new { controller = "Product", action = "Category" });

// PLP – All products
app.MapControllerRoute(
    name: "product-list",
    pattern: "san-pham",
    defaults: new { controller = "Product", action = "Category" });

// Search
app.MapControllerRoute(
    name: "search",
    pattern: "tim-kiem",
    defaults: new { controller = "Product", action = "Search" });

// Post detail by slug
app.MapControllerRoute(
    name: "post-detail",
    pattern: "bai-viet/{slug}",
    defaults: new { controller = "Post", action = "Detail" });

// Post listing
app.MapControllerRoute(
    name: "post-list",
    pattern: "bai-viet",
    defaults: new { controller = "Post", action = "Index" });

// Contact
app.MapControllerRoute(
    name: "contact",
    pattern: "lien-he",
    defaults: new { controller = "Contact", action = "Index" });

// Cart
app.MapControllerRoute(
    name: "cart",
    pattern: "gio-hang",
    defaults: new { controller = "Cart", action = "Index" });

// Track Order
app.MapControllerRoute(
    name: "track-order",
    pattern: "tra-cuu-don-hang",
    defaults: new { controller = "Cart", action = "TrackOrder" });

// Checkout
app.MapControllerRoute(
    name: "checkout",
    pattern: "thanh-toan",
    defaults: new { controller = "Checkout", action = "Index" });

// Checkout complete
app.MapControllerRoute(
    name: "checkout-complete",
    pattern: "thanh-toan/hoan-tat/{id}",
    defaults: new { controller = "Checkout", action = "Complete" });

// Default fallback route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
