using MediatR;
using ProductReviewer.Application.Common.Interface;

namespace ProductReviewer.Application.Segregation.Auth.Queries
{
    public record LoginRequestQuery : IRequest<string>
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class LoginRequestQueryHandler : IRequestHandler<LoginRequestQuery, string>
    {
        private readonly IAuthService _authService;
        public LoginRequestQueryHandler(IAuthService authService)
        {
            _authService = authService;
        }
        public Task<string> Handle(LoginRequestQuery request, CancellationToken cancellationToken)
        {
            return _authService.LoginValidate(request.Username, request.Password);
        }
    }
}
