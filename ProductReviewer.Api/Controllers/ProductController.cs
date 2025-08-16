using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductReviewer.Application.Segregation.Products.Queries;

namespace ProductReviewer.Api.Controllers
{
    public class ProductController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetAllProductsQuery query)
        {
            var result = await Mediator.Send(query);
            return Ok(result);
        }
    }
}
