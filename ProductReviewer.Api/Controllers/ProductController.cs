using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductReviewer.Application.Segregation.Products.Commands;
using ProductReviewer.Application.Segregation.Products.Queries;

namespace ProductReviewer.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : BaseApiController
    {
        [Authorize]
        [HttpGet("list")]
        public async Task<IActionResult> GetAll([FromQuery] GetAllProductsQuery query)
        {
            var result = await Mediator.Send(query);
            return Ok(result);
        }

        [Authorize(Policy = "AdminAndReviewer")]
        [HttpPost("create-review")]
        public async Task<IActionResult> ReviewProduct([FromBody] ReviewProductCommand command)
        {
            var result = await Mediator.Send(command);
            return Ok(result);
        }
    }
}
