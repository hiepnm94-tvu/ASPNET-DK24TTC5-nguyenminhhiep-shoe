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

// Add admin/manage route so requests to /admin map to ManageController
// Map routes including areas
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Manage}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();
