using FluentResults;
using Moq;
using ProductReviewer.Application.Common.Dtos;
using ProductReviewer.Application.Common.Interface;
using ProductReviewer.Application.Segregation.Products.Queries;
using ProductReviewer.Domain.Enums;

namespace ProductReviewer.Test.Application.Segregation.Product.Queries
{
    public class GetProductByIdQueryTest
    {
        private readonly Mock<IProductService> _productService;
        private readonly GetProductByQueryHandler _handler;
        public GetProductByIdQueryTest()
        {
            _productService = new Mock<IProductService>();
            _handler = new GetProductByQueryHandler(_productService.Object);
        }
        [Trait("Category", "ProductHandlers")]
        [Fact]
        public async Task Handle_GetsProductById_ReturnsSuccess()
        {
            var expectedProduct = new ProductDto { Id = 1, Name = "Phone", AverageRating = 4.5, Category = ProductCategory.Good };
            var expectedResult = Result.Ok(expectedProduct);
            _productService.Setup(x => x.GetById(1))
                .ReturnsAsync(expectedResult);
            var query = new GetProductByIdQuery { id = 1 };
            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal(expectedProduct.Name, result.Value.Name);
            _productService.Verify(s => s.GetById(1), Times.Once);
        }
        [Trait("Category", "ProductHandlers")]
        [Fact]
        public async Task Handle_WhenServiceFails_ReturnsFailure()
        {
            var expectedResult = Result.Fail("Failed to fetch product");
            _productService.Setup(x => x.GetById(1))
                .ReturnsAsync(expectedResult);
            var query = new GetProductByIdQuery { id = 1 };
            var result = await _handler.Handle(query, CancellationToken.None);
            Assert.False(result.IsSuccess);
            Assert.Equal("Failed to fetch product", result.Errors[0].Message);
            _productService.Verify(s => s.GetById(1), Times.Once);
        }   
    }
}
