using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace quanlybangiay.Controllers.Admin
{
    [Area("Admin")]
    [Authorize]
    public class ProductsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
