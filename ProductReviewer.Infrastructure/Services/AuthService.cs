using Microsoft.AspNetCore.Identity;
using ProductReviewer.Application.Common.Interface;
using ProductReviewer.Infrastructure.Data.Identity;

namespace ProductReviewer.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;
        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ITokenService tokenService
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }
        public async Task<string> LoginValidate(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
                return "Username does not exist";
            var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);

            if (!result.Succeeded)
                return "Incorrect Password";

            return _tokenService.Generate(user.Id, username);
        }
    }
}
