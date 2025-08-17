using Microsoft.AspNetCore.Identity;
using ProductReviewer.Application.Common.Dtos;
using ProductReviewer.Application.Common.Interface;
using ProductReviewer.Infrastructure.Data.Identity;

namespace ProductReviewer.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ITokenService _tokenService;
        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            ITokenService tokenService
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
        }

        public async Task<string> AssignRole(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return "User does not exist";

            var roleExists = await _roleManager.RoleExistsAsync(roleName);

            if (!roleExists)
                return "Role does not exist";

            var result = await _userManager.AddToRoleAsync(user, roleName);

            if (!result.Succeeded)
                return result.Errors.Select(e => e.Description).FirstOrDefault()!;

            return "Role assigned successfully";
        }

        public async Task<string> CreateUser(UserDto request)
        {
            var userByUsername = await _userManager.FindByNameAsync(request.Username);

            if(userByUsername != null)
                return "Username already exists";

            var userByEmail = await _userManager.FindByEmailAsync(request.Email);

            if(userByEmail != null)
                return "Email already exists";

            var newUser = new ApplicationUser
            {
                UserName = request.Username,
                DisplayName = request.DisplayName,
                Email = request.Email,
                PhoneNumber = request.PhoneNo
            };

            var result = await _userManager.CreateAsync(newUser, request.Password);
            if (!result.Succeeded)
                return result.Errors.Select(e => e.Description).FirstOrDefault()!;

            var roleResult = await _userManager.AddToRoleAsync(newUser, "User");
            if (!roleResult.Succeeded)
                return result.Errors.Select(e => e.Description).FirstOrDefault()!;

            return "Successfully added new user";
        }

        public async Task<string> LoginValidate(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
                return "Username does not exist";
            var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);

            if (!result.Succeeded)
                return "Incorrect Password";

            return await _tokenService.Generate(user.Id, username);
        }
    }
}
