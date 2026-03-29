using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace quanlybangiay.Controllers.Admin
{
    [Area("Admin")]
    [Authorize]
    public class ManageController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
