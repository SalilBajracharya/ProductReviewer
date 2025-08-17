using MediatR;
using ProductReviewer.Application.Common.Dtos;
using ProductReviewer.Application.Common.Interface;

namespace ProductReviewer.Application.Segregation.Products.Commands
{
    public class ReviewProductCommand : IRequest<string>
    {
        public int ProductInt { get; set; }
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
           var reviewProductDto = new ReviewProductDto
           {
               ProductInt = request.ProductInt,
               Rating = request.Rating,
               Comment = request.Comment
           };

            await _productService.AddProductReview(reviewProductDto, cancellationToken);
           return "Review added successfully";
        }
    }
}
