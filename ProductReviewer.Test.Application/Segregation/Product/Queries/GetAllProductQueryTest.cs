using FluentResults;
using Moq;
using ProductReviewer.Application.Common.Dtos;
using ProductReviewer.Application.Common.Helper;
using ProductReviewer.Application.Common.Interface;
using ProductReviewer.Application.Segregation.Products.Queries;
using ProductReviewer.Domain.Enums;

namespace ProductReviewer.Test.Application.Segregation.Product.Queries
{
    public class GetAllProductQueryTest
    {
        private readonly Mock<IProductService> _productService;
        private readonly GetAllProductQueryHandler _handler;

        public GetAllProductQueryTest()
        {
            _productService = new Mock<IProductService>();
            _handler = new GetAllProductQueryHandler(_productService.Object);
        }

        [Trait("Category", "ProductHandlers")]
        [Fact]
        public async Task Handle_GetsAllProduct_ReturnsSuccess()
        {
            var expectedProducts = new List<ProductDto>{
                new ProductDto { Id = 1, Name = "Phone", AverageRating = 4.5, Category = ProductCategory.Good },
                new ProductDto { Id = 2, Name = "Shoes", AverageRating = 2.0, Category = ProductCategory.Bad }
            };

            var paginatedResult = new PaginatedList<ProductDto>(expectedProducts, expectedProducts.Count, 1, 10);

            var expectedResult = Result.Ok(paginatedResult);

            _productService.Setup(x => x.GetAllAsync(1, 10, null))
                .ReturnsAsync(expectedResult);

            var query = new GetAllProductsQuery();

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal(paginatedResult.Items.Count, result.Value.Items.Count);
            Assert.Equal(paginatedResult.Items[0].Name, result.Value.Items[0].Name);

            _productService.Verify(s => s.GetAllAsync(1, 10, null), Times.Once);
        }

        [Trait("Category", "ProductHandlers")]
        [Fact]
        public async Task Handle_WhenServiceFails_ReturnsFailure()
        { 
            var expectedResult = Result.Fail("Failed to fetch products");

            _productService.Setup(x => x.GetAllAsync(1, 10, null))
                .ReturnsAsync(expectedResult);

            var query = new GetAllProductsQuery();

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.Equal("Failed to fetch products", result.Errors[0].Message);

            _productService.Verify(s => s.GetAllAsync(1, 10, null), Times.Once);
        }
    }
}
