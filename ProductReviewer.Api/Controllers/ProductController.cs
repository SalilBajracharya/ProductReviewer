using FluentResults.Extensions.AspNetCore;
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
        /// <summary>
        /// Retrieves a list of all products.
        /// </summary>
        /// <param name="query">Request contains pageno and pagesize for pagination purpose and ProductCategory "Worst", "Bad", "Good" for filtering</param>
        /// <returns>List of Products with reviews</returns>
        [Authorize]
        [HttpGet("list")]
        public async Task<IActionResult> GetAll([FromQuery] GetAllProductsQuery query)
        {
            var result = await Mediator.Send(query);
            return result.ToActionResult();
        }

        /// <summary>
        /// Retrieves a product by its ID.
        /// </summary>
        /// <param name="query">Request contains Product ID for fetching product by id</param>
        /// <returns>Returns a single product if success and failure message if there are no product the id provided</returns>
        [Authorize]
        [HttpGet("id")]
        public async Task<IActionResult> GetProductById([FromQuery] GetProductByIdQuery query)
        {
            var result = await Mediator.Send(query);
            return result.ToActionResult();
        }

        /// <summary>
        /// Creates a new product.
        /// </summary>
        /// <param name="command">Request contains Name, Description,SKU, ProductType</param>
        /// <returns>Returns sucess/failure message</returns>
        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command)
        {
            var result = await Mediator.Send(command);
            return result.ToActionResult();
        }

        /// <summary>
        /// Creates a review for a product for only "Admin"/"Reviewer" role
        /// </summary>
        /// <param name="command">Request contains productid, rating & comment</param>
        /// <returns>Returns sucess/failure message</returns>
        [Authorize(Policy = "AdminAndReviewer")]
        [HttpPost("create-review")]
        public async Task<IActionResult> ReviewProduct([FromBody] ReviewProductCommand command)
        {
            var result = await Mediator.Send(command);
            return result.ToActionResult();
        }

        /// <summary>
        /// Generates a CSV report file of product ratings.
        /// </summary>
        /// <param name="query">Request contains pageno and pagesize for pagination purpose and ProductCategory "Worst", "Bad", "Good" for filtering</param>
        /// <returns>CSV file with product list</returns>
        [Authorize]
        [HttpGet("products-report")]
        public async Task<IActionResult> DownloadProductRatingsReport([FromQuery] GenerateProductReportQuery query)
        {
            var result = await Mediator.Send(query);
            return File(result, "text/csv", "ProductsRatingReport.csv");
        }
    }
}
