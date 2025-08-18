using FluentResults;
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

        public async Task<Result<string>> AddProductReview(ReviewProductDto reviewProductDto)
        {
            var addReview = new Review
            {
                ProductId = reviewProductDto.ProductId,
                Rating = reviewProductDto.Rating,
                Comment = reviewProductDto.Comment ?? string.Empty,
                UserId = _currentUser.UserId!
            };

            await _ctx.Reviews.AddAsync(addReview);
            await _ctx.SaveChangesAsync();

            return Result.Ok("Added Review Successfully.");
        }


        public async Task<Result<bool>> CheckUserReviewExists(int productId)
        {
            var result = await _ctx.Reviews.AnyAsync(
                r => r.ProductId == productId && r.UserId == _currentUser.UserId);

            return Result.Ok(result);
        }

        public async Task<Result<string>> CreateProduct(CreateProductDto productdto)
        {
            var product = new Product
            {
                Name = productdto.Name,
                Description = productdto.Description,
                SKU = productdto.SKU,
                ProductType = productdto.ProductType
            };

            await _ctx.Products.AddAsync(product);
            await _ctx.SaveChangesAsync();

            return Result.Ok("Added Product successfully.");
        }

        public async Task<Result<List<ProductDto>>> GetAllAsync(ProductCategory? category = null)
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

            return Result.Ok(await query.ToListAsync());
        }
    }
}
