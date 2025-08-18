using FluentResults;
using Moq;
using ProductReviewer.Application.Common.Dtos;
using ProductReviewer.Application.Common.Interface;
using ProductReviewer.Application.Segregation.Products.Queries;
using ProductReviewer.Domain.Enums;
using System.Text;

namespace ProductReviewer.Test.Application.Segregation.Product.Queries
{
    public class GenerateProductReportQueryTest
    {
        private readonly Mock<IProductService> _productService;
        private readonly Mock<ICsvGenerator> _csvGenerator;
        private readonly GenerateProductReportQueryHandler _handler;

        public GenerateProductReportQueryTest()
        {
            _productService = new Mock<IProductService>();
            _csvGenerator = new Mock<ICsvGenerator>();
            _handler = new GenerateProductReportQueryHandler(_productService.Object, _csvGenerator.Object);
        }

        [Trait("Category", "ProductHandlers")]
        [Fact]
        public async Task Handle_ShouldReturnCsvBytes_WhenProductsExist()
        {
            var products = new List<ProductDto>
            {
               new ProductDto
                {
                    Id = 1,
                    Name = "Product A",
                    Description = "A test product",
                    SKU = "SKU001",
                    ProductType = "Electronics",
                    TotalReviews = 10,
                    AverageRating = 4.5,
                    Category = ProductCategory.Good
                },
                new ProductDto
                {
                    Id = 2,
                    Name = "Product B",
                    Description = "Another product",
                    SKU = "SKU002",
                    ProductType = "Books",
                    TotalReviews = 5,
                    AverageRating = 3.0,
                    Category = ProductCategory.Bad
                }
            };

            var csvString = "Id,Name,Description,SKU,ProductType,TotalReviews,AverageRating,Category" +
                            "1,Product A,A test product,SKU001,Electronics,10,4.5,Good" +
                            "2,Product B,Another product,SKU002,Books,5,3,Bad";

            var expectedBytes = Encoding.UTF8.GetBytes(csvString);

            _productService.Setup(s => s.GetAllAsync(null))
                 .ReturnsAsync(Result.Ok(products));

            _csvGenerator.Setup(g => g.Export(products))
                .Returns(csvString);

            var query = new GenerateProductReportQuery();
            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.Equal(expectedBytes, result);
            _productService.Verify(s => s.GetAllAsync(null), Times.Once);
            _csvGenerator.Verify(g => g.Export(products), Times.Once);

        }

        [Trait("Category", "ProductHandlers")]
        [Fact]
        public async Task Handle_ReturnEmptyCsv_WhenNoProducts()
        { 
            var emptyList = new List<ProductDto>();
            const string expectedHeader = "Id, Name, Description, SKU, ProductType, TotalReviews, AverageRating, Category";

            _productService.Setup(x => x.GetAllAsync(null))
                .ReturnsAsync(Result.Ok(emptyList));

            _csvGenerator.Setup(x => x.Export(emptyList))
                .Returns(expectedHeader);

            var result = await _handler.Handle(new GenerateProductReportQuery(), CancellationToken.None);

            Assert.NotNull(result);
            var resultString = Encoding.UTF8.GetString(result);
            Assert.Contains(expectedHeader, resultString);

            _productService.Verify(x => x.GetAllAsync(null), Times.Once);
            _csvGenerator.Verify(x => x.Export(emptyList), Times.Once);
        }
    }
}
