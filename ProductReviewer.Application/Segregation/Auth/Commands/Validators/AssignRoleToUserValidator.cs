using FluentValidation;
using ProductReviewer.Domain.Constants;

namespace ProductReviewer.Application.Segregation.Auth.Commands.Validators
{
    public class AssignRoleToUserValidator : AbstractValidator<AssignRoleToUserCommand>
    {
        public AssignRoleToUserValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required");

            RuleFor(x => x.RoleName)
                .NotEmpty().WithMessage("RoleName is required")
                .Must(role => UserRoles.AllRoles.Contains(role))
                .WithMessage($"Role must be one of : {string.Join(", ", UserRoles.AllRoles)}");
        }
    }
}
