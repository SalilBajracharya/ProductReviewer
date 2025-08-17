using ProductReviewer.Application.Common.Dtos;
using ProductReviewer.Domain.Entities;

namespace ProductReviewer.Application.Common.Interface
{
    public interface IProductService
    {
        Task<string> CreateProduct(Product product);
        Task<List<ProductDto>> GetAllAsync(CancellationToken cancellationToken);
        Task<bool> CheckUserReviewExists(int productId);
        Task AddProductReview(ReviewProductDto reviewProductDto);
    }
}
