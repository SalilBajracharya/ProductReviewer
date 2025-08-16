using Microsoft.EntityFrameworkCore;
using ProductReviewer.Application.Common.Dtos;
using ProductReviewer.Application.Common.Interface;
using ProductReviewer.Infrastructure.Data;

namespace ProductReviewer.Infrastructure.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _ctx;
        public ProductService(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }
        public async Task<List<ProductDto>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _ctx.Products
                .Include(p => p.Reviews)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    SKU = p.SKU,
                    ProductType = p.ProductType,
                    AverageRating = p.Reviews.Any() ? p.Reviews.Average(r => r.Rating) : 0.0,
                    Category = p.Reviews.Average(r => r.Rating) > 4.0 ? "High" : p.Reviews.Average(r => r.Rating) > 2.0 ? "Medium" : "Low"
                }).ToListAsync(cancellationToken);
        }
    }
}
