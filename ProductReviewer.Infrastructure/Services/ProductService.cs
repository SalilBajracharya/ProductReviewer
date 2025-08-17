using Microsoft.EntityFrameworkCore;
using ProductReviewer.Application.Common.Dtos;
using ProductReviewer.Application.Common.Interface;
using ProductReviewer.Domain.Entities;
using ProductReviewer.Domain.Enums;
using ProductReviewer.Infrastructure.Data;

namespace ProductReviewer.Infrastructure.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _ctx;
        private readonly ICurrentUser _currentUser;
        public ProductService(ApplicationDbContext ctx, ICurrentUser currentUser)
        {
            _ctx = ctx;
            _currentUser = currentUser;
        }

        public async Task AddProductReview(ReviewProductDto reviewProductDto)
        {
            var addReview = new Review
            {
                ProductId = reviewProductDto.ProductInt,
                Rating = reviewProductDto.Rating,
                Comment = reviewProductDto.Comment ?? string.Empty,
                UserId = _currentUser.UserId!
            };

            await _ctx.Reviews.AddAsync(addReview);
            await _ctx.SaveChangesAsync();
        }

        public async Task<bool> CheckUserReviewExists(int productId)
        {
            return await _ctx.Reviews.AnyAsync(
                r => r.ProductId == productId && r.UserId == _currentUser.UserId);
        }

        public async Task<string> CreateProduct(Product product)
        {
            await _ctx.Products.AddAsync(product);
            await _ctx.SaveChangesAsync();

            return "Product created successfully";
        }

        public async Task<List<ProductDto>> GetAllAsync(ProductCategory? category = null)
        {
            var query = _ctx.Products
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    SKU = p.SKU,
                    ProductType = p.ProductType,
                    AverageRating = p.Reviews.Any() ? p.Reviews.Average(r => r.Rating) : 0.0,
                    Category =
                        p.Reviews.Any() && p.Reviews.Average(r => r.Rating) >= 4.0 ? ProductCategory.Good :
                        p.Reviews.Any() && p.Reviews.Average(r => r.Rating) >= 2.0 ? ProductCategory.Bad :
                        ProductCategory.Worst
                });

            if (category.HasValue)
            {
                query = query.Where(p => p.Category == category.Value);
            }

            return await query.ToListAsync();
        }
    }
}
