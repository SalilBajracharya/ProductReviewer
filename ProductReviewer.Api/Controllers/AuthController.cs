using Microsoft.AspNetCore.Mvc;
using ProductReviewer.Application.Segregation.Auth.Queries;

namespace ProductReviewer.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : BaseApiController
    {
        [HttpPost]
        public async Task<IActionResult> Authenticate(LoginRequestQuery query)
        {
            var result = await Mediator.Send(query);
            return Ok(result);
        }
    }
}
