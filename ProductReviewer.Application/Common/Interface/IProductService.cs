using ProductReviewer.Application.Common.Dtos;

namespace ProductReviewer.Application.Common.Interface
{
    public interface IProductService
    {
        Task<List<ProductDto>> GetAllAsync(CancellationToken cancellationToken);
        Task AddProductReview(ReviewProductDto reviewProductDto, CancellationToken cancellationToken);
    }
}
