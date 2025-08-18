using MediatR;
using ProductReviewer.Application.Common.Interface;
using ProductReviewer.Domain.Enums;
using System.Text;

namespace ProductReviewer.Application.Segregation.Products.Queries
{
    public class GenerateProductReportQuery : IRequest<byte[]>
    {
        public ProductCategory? Category { get; set; }
    }

    public class GenerateProductReportQueryHandler : IRequestHandler<GenerateProductReportQuery, byte[]>
    {
        private readonly IProductService _productService;
        private readonly ICsvGenerator _csvGenerator;
        public GenerateProductReportQueryHandler(IProductService productService, ICsvGenerator csvGenerator)
        {
            _productService = productService;
            _csvGenerator = csvGenerator;
        }
        public async Task<byte[]> Handle(GenerateProductReportQuery request, CancellationToken cancellationToken)
        {
            var products = await _productService.GetAllAsync(request.Category);
            var csv = _csvGenerator.Export(products.Value);
            return Encoding.UTF8.GetBytes(csv);
        }
    }
}
