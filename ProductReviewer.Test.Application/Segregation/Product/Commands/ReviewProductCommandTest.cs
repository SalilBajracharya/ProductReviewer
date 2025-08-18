using FluentResults;
using Moq;
using ProductReviewer.Application.Common.Dtos;
using ProductReviewer.Application.Common.Interface;
using ProductReviewer.Application.Segregation.Products.Commands;

namespace ProductReviewer.Test.Application.Segregation.Product.Commands
{
    public class ReviewProductCommandTest
    {
        private readonly Mock<IProductService> _productService;
        public readonly ReviewProductCommandHandler _handler;

        public ReviewProductCommandTest()
        {
            _productService = new Mock<IProductService>();
            _handler = new ReviewProductCommandHandler(_productService.Object);
        }

        [Trait("Category", "ProductHandlers")]
        [Fact]
        public async Task Handle_CallReviewProduct_ReturnsSuccess()
        {
            var command = new ReviewProductCommand
            {
                ProductId = 1,
                Rating = 4.5,
                Comment = "Great product!"
            };
            var expectedResult = Result.Ok("Review added successfully");

            _productService.Setup(x => x.CheckUserReviewExists(command.ProductId))
                .ReturnsAsync(Result.Ok(false));

            _productService.Setup(x => x.AddProductReview(It.IsAny<ReviewProductDto>()))
                .ReturnsAsync(expectedResult);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal(expectedResult.Value, result.Value);

            _productService.Verify(x => x.CheckUserReviewExists(command.ProductId), Times.Once);
            _productService.Verify(x => x.AddProductReview(It.Is<ReviewProductDto>(r =>
                r.ProductId == command.ProductId &&
                r.Rating == command.Rating &&
                r.Comment == command.Comment)), Times.Once);
        }


        [Trait("Category", "ProductHandlers")]
        [Fact]
        public async Task Handle_WhenUserHasReviewed_ReturnsFailure()
        {
            var command = new ReviewProductCommand
            {
                ProductId = 1,
                Rating = 4.5,
                Comment = "Second review !"
            };
            var expectedResult = "You have already reviewed this product";

            _productService.Setup(x => x.CheckUserReviewExists(command.ProductId))
                .ReturnsAsync(Result.Ok(true));

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.Equal(expectedResult, result.Errors[0].Message);

            _productService.Verify(x => x.CheckUserReviewExists(command.ProductId), Times.Once);
            _productService.Verify(x => x.AddProductReview(It.IsAny<ReviewProductDto>()), Times.Never);
        }


    }
}
