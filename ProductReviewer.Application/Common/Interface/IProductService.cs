using FluentResults;
using ProductReviewer.Application.Common.Dtos;
using ProductReviewer.Application.Common.Helper;
using ProductReviewer.Domain.Entities;
using ProductReviewer.Domain.Enums;

namespace ProductReviewer.Application.Common.Interface
{
    public interface IProductService
    {
        Task<Result<string>> CreateProduct(CreateProductDto product);
        Task<Result<PaginatedList<ProductDto>>> GetAllAsync(int pageNo, int pageSize, ProductCategory? category = null);
        Task<Result<bool>> CheckUserReviewExists(int productId);
        Task<Result<string>> AddProductReview(ReviewProductDto reviewProductDto);
    }
}
