using FluentResults;
using Moq;
using ProductReviewer.Application.Common.Interface;
using ProductReviewer.Application.Segregation.Auth.Queries;

namespace ProductReviewer.Test.Application.Segregation.Auth.Queries
{
    public class LoginRequestQueryHandlerTest
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly LoginRequestQueryHandler _handler;

        public LoginRequestQueryHandlerTest()
        {
            _authServiceMock = new Mock<IAuthService>();
            _handler = new LoginRequestQueryHandler(_authServiceMock.Object);
        }

        [Trait("Category", "LoginRequestHandler")]
        [Fact]
        public async Task Handle_CallsAuthServiceLoginValidate_ReturnsToken()
        {
            var query = new LoginRequestQuery
            {
                Username = "testuser",
                Password = "testpassword"
            };

            var expectedToken = Result.Ok("token123");

            _authServiceMock.Setup(x => x.LoginValidate(query.Username, query.Password))
                .ReturnsAsync(expectedToken);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal(expectedToken, result);
            _authServiceMock.Verify(x => x.LoginValidate(query.Username, query.Password), Times.Once);
        }

        [Trait("Category", "LoginRequestHandler")]
        [Fact]
        public async Task Handle_CallsAuthServiceLoginValidate_ReturnsFailureResult()
        {
            var query = new LoginRequestQuery
            {
                Username = "testuser",
                Password = "wrongpassword"
            };

            var failure = Result.Fail<string>("Invalid credentials");
            _authServiceMock.Setup(x => x.LoginValidate(query.Username, query.Password))
                .ReturnsAsync(failure);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.True(result.IsFailed);
            Assert.Contains(result.Errors, e => e.Message == "Invalid credentials"); 
        }
    }
}
