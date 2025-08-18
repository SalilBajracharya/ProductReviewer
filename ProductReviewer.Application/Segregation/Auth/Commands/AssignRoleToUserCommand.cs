using FluentResults;
using MediatR;
using ProductReviewer.Application.Common.Interface;

namespace ProductReviewer.Application.Segregation.Auth.Commands
{
    public class AssignRoleToUserCommand : IRequest<Result<string>>
    {
        public string UserId { get; set; } = string.Empty;
        public string RoleName { get; set; } = "Reviewer";
    }

    public class AssignRoleToUserCommandHandler : IRequestHandler<AssignRoleToUserCommand, Result<string>>
    {
        private readonly IAuthService _authService;
        public AssignRoleToUserCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }
        public async Task<Result<string>> Handle(AssignRoleToUserCommand request, CancellationToken cancellationToken)
        {
            return await _authService.AssignRole(request.UserId, request.RoleName);
        }
    }
}
