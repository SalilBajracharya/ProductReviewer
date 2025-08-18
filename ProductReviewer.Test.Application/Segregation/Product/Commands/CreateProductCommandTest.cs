using FluentResults;
using Moq;
using ProductReviewer.Application.Common.Dtos;
using ProductReviewer.Application.Common.Interface;
using ProductReviewer.Application.Segregation.Products.Commands;

namespace ProductReviewer.Test.Application.Segregation.Product.Commands
{
    public class CreateProductCommandTest
    {
        private readonly Mock<IProductService> _productServiceMock;
        private readonly CreateProductCommandHandler _handler;

        public CreateProductCommandTest()
        {
            _productServiceMock = new Mock<IProductService>();
            _handler = new CreateProductCommandHandler(_productServiceMock.Object);
        }

        [Trait("Category", "ProductHandlers")]
        [Fact]
        public async Task Handle_CallCreateProduct_ReturnsSuccess()
        { 
            var command = new CreateProductCommand
            {
                Name = "Test Product",
                Description = "This is a test product",
                SKU = "TEST123",
                ProductType = "Electronics"
            };
            var expectedResult = Result.Ok("Added Product successfully.");
            
            _productServiceMock.Setup(x => x.CreateProduct(It.IsAny<CreateProductDto>()))
                .ReturnsAsync(expectedResult);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal(expectedResult.Value, result.Value);

            _productServiceMock.Verify(x => x.CreateProduct(It.Is<CreateProductDto>(p =>
                p.Name == command.Name &&
                p.Description == command.Description &&
                p.SKU == command.SKU &&
                p.ProductType == command.ProductType)), Times.Once);
        }

        [Trait("Category", "ProductHandlers")]
        [Fact]
        public async Task Handle_CreateProductWithDuplicateSKU_ReturnsFailure()
        {
            var command = new CreateProductCommand
            {
                Name = "Test Product",
                Description = "This is a test product",
                SKU = "DuplicateSKU",
                ProductType = "Electronics"
            };
            var expectedResult = Result.Fail("SKU already exists");

            _productServiceMock.Setup(x => x.CreateProduct(It.IsAny<CreateProductDto>()))
                .ReturnsAsync(expectedResult);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.Single(result.Errors);
            Assert.Equal("SKU already exists", result.Errors[0].Message);

            _productServiceMock.Verify(x => x.CreateProduct(It.Is<CreateProductDto>(p =>
                p.Name == command.Name &&
                p.Description == command.Description &&
                p.SKU == command.SKU &&
                p.ProductType == command.ProductType)), Times.Once);
        }

    }
}
