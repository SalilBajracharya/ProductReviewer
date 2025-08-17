using ProductReviewer.Application.Common.Dtos;
using ProductReviewer.Domain.Entities;
using ProductReviewer.Domain.Enums;

namespace ProductReviewer.Application.Common.Interface
{
    public interface IProductService
    {
        Task<string> CreateProduct(Product product);
        Task<List<ProductDto>> GetAllAsync(ProductCategory? category = null);
        Task<bool> CheckUserReviewExists(int productId);
        Task AddProductReview(ReviewProductDto reviewProductDto);
    }
}
