using MediatR;
using ProductReviewer.Application.Common.Interface;
using ProductReviewer.Domain.Enums;
using System.ComponentModel;
using System.Text;

namespace ProductReviewer.Application.Segregation.Products.Queries
{
    public class GenerateProductReportQuery : IRequest<byte[]>
    {
        [DefaultValue(1)]
        public int pageNo { get; set; } = 1;
        [DefaultValue(10)]
        public int pageSize { get; set; } = 10;
        [DefaultValue(null)]
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
            var products = await _productService.GetAllAsync(request.pageNo, request.pageSize, request.Category);
            var csv = _csvGenerator.Export(products.Value.Items);
            return Encoding.UTF8.GetBytes(csv);
        }
    }
}
