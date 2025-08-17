using MediatR;
using ProductReviewer.Application.Common.Dtos;
using ProductReviewer.Application.Common.Interface;

namespace ProductReviewer.Application.Segregation.Products.Commands
{
    public class ReviewProductCommand : IRequest<string>
    {
        public int ProductId { get; set; }
        public double Rating { get; set; }
        public string? Comment { get; set; } = string.Empty;
    }

    public class ReviewProductCommandHandler : IRequestHandler<ReviewProductCommand, string>
    {
        private readonly IProductService _productService;
        public ReviewProductCommandHandler(IProductService productService)
        {
            _productService = productService;
        }
        public async Task<string> Handle(ReviewProductCommand request, CancellationToken cancellationToken)
        {
            var userProductReviewExists = await _productService.CheckUserReviewExists(request.ProductInt);

            if (userProductReviewExists)
            {
                return "You have already reviewed this product";
            }

            var reviewProductDto = new ReviewProductDto
            {
                ProductInt = request.ProductInt,
                Rating = request.Rating,
                Comment = request.Comment
            };

            await _productService.AddProductReview(reviewProductDto);
            return "Review added successfully";
        }
    }
}
