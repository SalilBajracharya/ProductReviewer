using FluentResults;
using MediatR;
using ProductReviewer.Application.Common.Dtos;
using ProductReviewer.Application.Common.Interface;

namespace ProductReviewer.Application.Segregation.Products.Queries
{
    public class GetProductByIdQuery : IRequest<Result<ProductDto>>
    {
        public int id { get; set; }
    }

    public class GetProductByQueryHandler : IRequestHandler<GetProductByIdQuery, Result<ProductDto>>
    {
        private readonly IProductService _productService;
        public GetProductByQueryHandler(IProductService productService)
        {
            _productService = productService;
        }
        public async Task<Result<ProductDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            return await _productService.GetById(request.id);
        }
    }
}
