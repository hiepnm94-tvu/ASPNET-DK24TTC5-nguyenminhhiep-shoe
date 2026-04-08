using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace quanlybangiay.Controllers.Admin
{
    [Area("Admin")]
    public class ErrorController : Controller
    {
        [Route("Admin/Error/{statusCode}")]
        public IActionResult Handle(int statusCode)
        {
            return statusCode switch
            {
                404 => View("NotFound"),
                503 => View("Maintenance"),
                _ => View("Error", statusCode)
            };
        }


        [Route("Admin/Error")]
        public IActionResult Generic()
        {
            var featurePath = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            ViewBag.Path = featurePath?.Path ?? "-";
            ViewBag.ErrorMessage = featurePath?.Error.Message ?? "Đã xảy ra lỗi không mong muốn.";
            return View("Error", 500);
        }
    }
}
