using FluentResults;
using Moq;
using ProductReviewer.Application.Common.Dtos;
using ProductReviewer.Application.Common.Interface;
using ProductReviewer.Application.Segregation.Auth.Commands;

namespace ProductReviewer.Test.Application.Segregation.Auth.Commands
{
    public class CreateUserCommandTest
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly CreateUserCommandHandler _handler;

        public CreateUserCommandTest()
        {
            _authServiceMock = new Mock<IAuthService>();
            _handler = new CreateUserCommandHandler(_authServiceMock.Object);
        }

        [Trait("Category", "UserCommandHandler")]
        [Fact]
        public async Task Handle_CallsAuthServiceCreateUser_ReturnsSuccess()
        {
            var command = CreateUserCommand("newuser1", "Strong@123", "test@gmail.com", "New User 1");
            var expectedResult = Result.Ok("User created successfully");

            _authServiceMock.Setup(x => x.CreateUser(It.IsAny<UserDto>()))
                .ReturnsAsync(expectedResult);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal(expectedResult.Value, result.Value);

            _authServiceMock.Verify(x => x.CreateUser(It.Is<UserDto>(u =>
                u.Username == command.Username &&
                u.Password == command.Password &&
                u.Email == command.Email &&
                u.DisplayName == command.DisplayName)), Times.Once);
        }

        [Trait("Category", "UserCommandHandler")]
        [Fact]
        public async Task Handle_WhenUsernameExists_ReturnsFailure()
        {
            var command = CreateUserCommand("existinguser1", "Strong@123", "test@gmail.com", "new User 1");

            _authServiceMock.Setup(x => x.CreateUser(It.IsAny<UserDto>()))
                .ReturnsAsync(Result.Fail("Username already exists"));

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.Single(result.Errors);
            Assert.Equal("Username already exists", result.Errors[0].Message);

            _authServiceMock.Verify(x => x.CreateUser(It.Is<UserDto>(u =>
                u.Username == command.Username &&
                u.Password == command.Password &&
                u.Email == command.Email &&
                u.DisplayName == command.DisplayName)), Times.Once);
        }

        [Trait("Category", "UserCommandHandler")]
        [Fact]
        public async Task Handle_WhenEmailExists_ReturnsFailure()
        {
            var command = CreateUserCommand("newuser1", "Strong@123", "duplicate@gmail.com", "new User 1");


            _authServiceMock.Setup(x => x.CreateUser(It.IsAny<UserDto>()))
                .ReturnsAsync(Result.Fail("Email already exists"));

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.Single(result.Errors);
            Assert.Equal("Email already exists", result.Errors[0].Message);

            _authServiceMock.Verify(x => x.CreateUser(It.Is<UserDto>(u =>
                u.Username == command.Username &&
                u.Password == command.Password &&
                u.Email == command.Email &&
                u.DisplayName == command.DisplayName)), Times.Once);
        }

        [Trait("Category", "UserCommandHandler")]
        [Fact]
        public async Task Handle_WhenUserCreationFails_ReturnsFailure()
        {
            var command = CreateUserCommand("newuser1", "weakpw", "test@gmail.com", "new User 1");


            _authServiceMock.Setup(x => x.CreateUser(It.IsAny<UserDto>()))
                .ReturnsAsync(Result.Fail("Password validation fails."));

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.Single(result.Errors);
            Assert.Equal("Password validation fails.", result.Errors[0].Message);

            _authServiceMock.Verify(x => x.CreateUser(It.Is<UserDto>(u =>
                u.Username == command.Username &&
                u.Password == command.Password &&
                u.Email == command.Email &&
                u.DisplayName == command.DisplayName)), Times.Once);
        }

        [Trait("Category", "UserCommandHandler")]
        [Fact]
        public async Task Handle_WhenRoleAssignmentFails_ReturnsFailure()
        {
            var command = CreateUserCommand("newuser1", "Strong@123", "test@gmail.com", "new User 1");

            _authServiceMock.Setup(x => x.CreateUser(It.IsAny<UserDto>()))
                .ReturnsAsync(Result.Fail("Role assignment fails."));

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.Single(result.Errors);
            Assert.Equal("Role assignment fails.", result.Errors[0].Message);

            _authServiceMock.Verify(x => x.CreateUser(It.Is<UserDto>(u =>
                u.Username == command.Username &&
                u.Password == command.Password &&
                u.Email == command.Email &&
                u.DisplayName == command.DisplayName)), Times.Once);
        }

        private CreateUserCommand CreateUserCommand(string username, string password, string email, string displayName)
        {
            return new CreateUserCommand
            {
                Username = username,
                Password = password,
                Email = email,
                DisplayName = displayName
            };

        }
    }
}
