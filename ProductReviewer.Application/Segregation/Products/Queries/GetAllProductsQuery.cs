using FluentResults;
using MediatR;
using ProductReviewer.Application.Common.Dtos;
using ProductReviewer.Application.Common.Helper;
using ProductReviewer.Application.Common.Interface;
using ProductReviewer.Domain.Enums;
using System.ComponentModel;

namespace ProductReviewer.Application.Segregation.Products.Queries
{
    public class GetAllProductsQuery : IRequest<Result<PaginatedList<ProductDto>>>
    {
        [DefaultValue(1)]
        public int pageNo { get; set; } = 1;
        [DefaultValue(10)]
        public int pageSize { get; set; } = 10;
        [DefaultValue(null)]
        public ProductCategory? Category { get; set; } = null;

    }

    public class GetAllProductQueryHandler : IRequestHandler<GetAllProductsQuery, Result<PaginatedList<ProductDto>>>
    {
        private readonly IProductService _productService;
        public GetAllProductQueryHandler(IProductService productService)
        {
            _productService = productService;
        }
        public async Task<Result<PaginatedList<ProductDto>>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            return await _productService.GetAllAsync(request.pageNo, request.pageSize, request.Category);
        }
    }
}