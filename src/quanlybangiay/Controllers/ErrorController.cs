using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace quanlybangiay.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/{statusCode}")]
        public IActionResult Handle(int statusCode)
        {
            return statusCode switch
            {
                404 => View("NotFound"),
                _ => View("Error", statusCode)
            };
        }

        [Route("Error")]
        public IActionResult Generic()
        {
            var feature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            ViewBag.Path = feature?.Path ?? "—";
            ViewBag.Exception = feature?.Error?.Message ?? "Đã xảy ra lỗi không mong muốn";
            return View("Error", 500);
        }
    }
}
