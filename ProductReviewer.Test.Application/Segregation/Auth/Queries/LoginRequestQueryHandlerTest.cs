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
        public async Task Handle_LoginValidateWithInvalidUser_ReturnsFailureResult()
        {
            var query = new LoginRequestQuery
            {
                Username = "nonexistentuser",
                Password = "Strong@123"
            };

            var failure = Result.Fail<string>("Username does not exist");
            _authServiceMock.Setup(x => x.LoginValidate(query.Username, query.Password))
                .ReturnsAsync(failure);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.True(result.IsFailed);
            Assert.Contains("Username does not exist", result.Errors[0].Message); 

            _authServiceMock.Verify(x => x.LoginValidate(query.Username, query.Password), Times.Once);
        }

        [Trait("Category", "LoginRequestHandler")]
        [Fact]
        public async Task Handle_LoginValidateWithInvalidPassword_ReturnsFailureResult()
        {
            var query = new LoginRequestQuery
            {
                Username = "newuser1",
                Password = "wrongpassword"
            };

            var failure = Result.Fail<string>("Incorrect Password");
            _authServiceMock.Setup(x => x.LoginValidate(query.Username, query.Password))
                .ReturnsAsync(failure);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.True(result.IsFailed);
            Assert.Contains("Incorrect Password", result.Errors[0].Message);

            _authServiceMock.Verify(x => x.LoginValidate(query.Username, query.Password), Times.Once);
        }
    }
}
