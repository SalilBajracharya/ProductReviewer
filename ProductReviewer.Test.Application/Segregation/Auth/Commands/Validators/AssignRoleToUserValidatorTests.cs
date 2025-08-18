using FluentValidation.TestHelper;
using ProductReviewer.Application.Segregation.Auth.Commands;
using ProductReviewer.Application.Segregation.Auth.Commands.Validators;

namespace ProductReviewer.Test.Application.Segregation.Auth.Commands.Validators
{
    public class AssignRoleToUserValidatorTests
    {
        private readonly AssignRoleToUserValidator _validator;
        public AssignRoleToUserValidatorTests()
        {
            _validator = new AssignRoleToUserValidator();
        }

        [Fact]
        public void ShouldReturnError_WhenUserIdIsEmpty()
        {
            var command = CreateCommand("", "Admin");
            
            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.UserId);
        }

        [Fact]
        public void ShouldReturnError_WhenRoleNameIsEmpty()
        {
            var command = CreateCommand("User123", "");

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.RoleName);
        }

        [Fact]
        public void ShouldReturnError_WhenRoleNameIsInvalid()
        {
            var command = CreateCommand("User123", "Worker");

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.RoleName);
        }

        [Fact]
        public void ShouldNotReturnError_WhenValid()
        {
            var command = CreateCommand("User123", "Reviewer");

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        private AssignRoleToUserCommand CreateCommand(string userId, string roleName)
        {
            return new AssignRoleToUserCommand
            {
                UserId = userId,
                RoleName = roleName
            };
        }
    }
}
