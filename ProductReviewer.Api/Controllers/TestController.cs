using Microsoft.AspNetCore.Mvc;

namespace ProductReviewer.Api.Controllers
{
    public class TestController : BaseApiController
    {
        [HttpGet("Crash")]
        public IActionResult Index()
        {
            throw new Exception("this is a test crash");
        }
    }
}
