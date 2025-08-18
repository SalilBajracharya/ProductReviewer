using FluentResults;
using MediatR;
using ProductReviewer.Application.Common.Interface;

namespace ProductReviewer.Application.Segregation.Auth.Queries
{
    public record LoginRequestQuery : IRequest<Result<string>>
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginRequestQueryHandler : IRequestHandler<LoginRequestQuery, Result<string>>
    {
        private readonly IAuthService _authService;
        public LoginRequestQueryHandler(IAuthService authService)
        {
            _authService = authService;
        }
        public async Task<Result<string>> Handle(LoginRequestQuery request, CancellationToken cancellationToken)
        {
            return await _authService.LoginValidate(request.Username, request.Password);
        }
    }
}
