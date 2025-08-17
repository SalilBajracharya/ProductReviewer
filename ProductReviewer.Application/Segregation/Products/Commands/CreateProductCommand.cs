using MediatR;
using ProductReviewer.Application.Common.Interface;
using ProductReviewer.Domain.Entities;

namespace ProductReviewer.Application.Segregation.Products.Commands
{
    public class CreateProductCommand : IRequest<string>
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public string ProductType { get; set; } = string.Empty;
    }

    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, string>
    {
        private readonly IProductService _productService;
        public CreateProductCommandHandler(IProductService productService)
        {
            _productService = productService;
        }
        public async Task<string> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                SKU = request.SKU,
                ProductType = request.ProductType
            };

            return await _productService.CreateProduct(product);
        }
    }
}
