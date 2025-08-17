using MediatR;
using ProductReviewer.Application.Common.Dtos;
using ProductReviewer.Application.Common.Interface;

namespace ProductReviewer.Application.Segregation.Auth.Commands
{
    public record CreateUserCommand : IRequest<string>
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
    }

    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, string>
    {
        private readonly IAuthService _authService;
        public CreateUserCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }
        public async Task<string> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var dto = new UserDto
            {
                Username = request.Username,
                Email = request.Email,
                Password = request.Password,
                DisplayName = request.DisplayName
            };

            var result = await _authService.CreateUser(dto);
            return result;
        }
    }
}
