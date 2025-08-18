using FluentResults;
using Microsoft.AspNetCore.Identity;
using ProductReviewer.Application.Common.Dtos;
using ProductReviewer.Application.Common.Interface;
using ProductReviewer.Infrastructure.Data.Identity;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        public async Task<Result<string>> AssignRole(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return Result.Fail("User does not exist");

            var roleExists = await _roleManager.RoleExistsAsync(roleName);

            if (!roleExists)
                return Result.Fail("Role does not exist");

            var result = await _userManager.AddToRoleAsync(user, roleName);

            if (!result.Succeeded)
                return Result.Fail(result.Errors.Select(e => e.Description));

            return Result.Ok("Role assigned successfully");
        }

        public async Task<Result<string>> CreateUser(UserDto request)
        {
            var userByUsername = await _userManager.FindByNameAsync(request.Username);

            if(userByUsername != null)
                return Result.Fail("Username already exists");

            var userByEmail = await _userManager.FindByEmailAsync(request.Email);

            if(userByEmail != null)
                return Result.Fail("Email already exists");

            var newUser = new ApplicationUser
            {
                UserName = request.Username,
                DisplayName = request.DisplayName,
                Email = request.Email,
                PhoneNumber = request.PhoneNo
            };

            var result = await _userManager.CreateAsync(newUser, request.Password);
            if (!result.Succeeded)
                return Result.Fail(result.Errors.Select(e => e.Description));

            var roleResult = await _userManager.AddToRoleAsync(newUser, "User");
            if (!roleResult.Succeeded)
                return Result.Fail(result.Errors.Select(e => e.Description));

            return Result.Ok("Successfully added new user");
        }

        public async Task<Result<string>> LoginValidate(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
                return Result.Fail("Username does not exist");
            var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);

            if (!result.Succeeded)
                return Result.Fail("Incorrect Password");

            return Result.Ok(await _tokenService.Generate(user.Id, username));
        }
    }
}
