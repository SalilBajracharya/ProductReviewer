using ProductReviewer.Application.Segregation.Auth.Commands;
using FluentValidation.TestHelper;
using ProductReviewer.Application.Segregation.Auth.Commands.Validators;

namespace ProductReviewer.Test.Application.Segregation.Auth.Commands.Validators
{
    public class CreateUserCommandValidatorTests
    {
        private readonly CreateUserCommandValidator _validator;
        public CreateUserCommandValidatorTests()
        {
            _validator = new CreateUserCommandValidator();
        }


        [Theory]
        [InlineData("", "Username is required")]
        [InlineData("a", "Username must be at least 3 characters long")]
        [InlineData("a1234567890b12345678901", "Username must not exceed 20 characters")]
        public void ShouldReturnError_WhenInvalidUsername(string username, string expectedMessage)
        {
            var command = CreateUser(username, "Strong@123", "test@gmail.com", "Test User 1");
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Username)
                .WithErrorMessage(expectedMessage);
        }

        [Theory]
        [InlineData("", "Password is required")]
        [InlineData("abc@", "Password must be at least 6 characters long")]
        [InlineData("a1234567!8", "Password must contain at least one uppercase letter")]
        [InlineData("ABCDERFAS12!3", "Password must contain at least one lowercase letter")]
        [InlineData("ABCDgsAS", "Password must contain at least one number")]
        [InlineData("ABCgAS", "Password must contain at least one special character")]

        public void ShouldReturnError_WhenInvalidPassword(string password, string expectedMessage)
        {
            var command = CreateUser("testuser1", password, "test@gmail.com", "Test User 1");
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Password)
                .WithErrorMessage(expectedMessage);
        }

        [Theory]
        [InlineData("", "Email is required")]
        [InlineData("abscdgmail", "Invalid email format")]
        public void ShouldReturnError_WhenInvalidEmail(string email, string expectedMessage)
        {
            var command = CreateUser("testuser1", "Strong@123", email, "Test User 1");
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Email)
                .WithErrorMessage(expectedMessage);
        }

        [Theory]
        [InlineData(0, "Display Name is required")]
        [InlineData(1, "Display Name must be at least 3 characters long")]
        [InlineData(51, "Display Name must not exceed 50 characters")]
        public void ShouldReturnError_WhenInvalidDisplayName(int length, string expectedMessage)
        {
            string displayName = new string('a', length);
            var command = CreateUser("testuser1", "Strong@123", "test@gmail.com", displayName);
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.DisplayName)
                .WithErrorMessage(expectedMessage);
        }

        public void ShouldSucceed_WhenValid()
        {
            var command = CreateUser("Test User1", "Strong@123", "test@gmail.com", "Hello World User");
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }

        private CreateUserCommand CreateUser(string username, string password, string email, string displayName)
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
