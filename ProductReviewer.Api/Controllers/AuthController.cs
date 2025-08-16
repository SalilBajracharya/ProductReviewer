using Microsoft.AspNetCore.Mvc;

namespace ProductReviewer.Api.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
