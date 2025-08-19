using FluentResults;
using MediatR;
using ProductReviewer.Application.Common.Dtos;
using ProductReviewer.Application.Common.Interface;
using ProductReviewer.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace ProductReviewer.Application.Segregation.Products.Commands
{
    public class CreateProductCommand : IRequest<Result<string>>
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string? Description { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public string? ProductType { get; set; } = string.Empty;
    }

    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<string>>
    {
        private readonly IProductService _productService;
        public CreateProductCommandHandler(IProductService productService)
        {
            _productService = productService;
        }
        public async Task<Result<string>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var product = new CreateProductDto
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
