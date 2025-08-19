using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using ProductReviewer.Application.Common.Interface;
using ProductReviewer.Infrastructure.Data.Identity;
using ProductReviewer.Infrastructure.Services;

namespace ProductReviewer.Test.Infrastructure.Services
{
    public class AuthServiceTest
    {
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<SignInManager<ApplicationUser>> _signInManagerMock;
        private readonly Mock<RoleManager<IdentityRole>> _roleManagerMock;
        private readonly Mock<ITokenService> _tokenServiceMock;

        private readonly AuthService _authService;

        public AuthServiceTest()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            //_userManagerMock = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(
                            userStoreMock.Object,
                            Mock.Of<IOptions<IdentityOptions>>(),
                            Mock.Of<IPasswordHasher<ApplicationUser>>(),
                            new IUserValidator<ApplicationUser>[0],
                            new IPasswordValidator<ApplicationUser>[0],
                            Mock.Of<ILookupNormalizer>(),
                            new IdentityErrorDescriber(),
                            Mock.Of<IServiceProvider>(),
                            Mock.Of<ILogger<UserManager<ApplicationUser>>>()
                        );

            var contextAccessorMock = new Mock<IHttpContextAccessor>();

            var userClaimsPrincipalFactoryMock = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();

            var contextAccessor = new Mock<IHttpContextAccessor>();
            var claimsFactory = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            var options = new Mock<IOptions<IdentityOptions>>();
            var logger = new Mock<ILogger<SignInManager<ApplicationUser>>>();
            var schemeProvider = new Mock<IAuthenticationSchemeProvider>();
            _signInManagerMock = new Mock<SignInManager<ApplicationUser>>(
                            _userManagerMock.Object,
                            contextAccessor.Object,
                            claimsFactory.Object,
                            options.Object,
                            logger.Object,
                            schemeProvider.Object
                        )
            { CallBase = true };
            var roleStoreMock = new Mock<IRoleStore<IdentityRole>>();
            //_roleManagerMock = new Mock<RoleManager<IdentityRole>>(roleStoreMock.Object, null, null, null, null);

            _roleManagerMock = new Mock<RoleManager<IdentityRole>>(
                               roleStoreMock.Object,
                               new IRoleValidator<IdentityRole>[0],
                               Mock.Of<ILookupNormalizer>(),
                               new IdentityErrorDescriber(),
                               Mock.Of<ILogger<RoleManager<IdentityRole>>>());


           _tokenServiceMock = new Mock<ITokenService>();

            _authService = new AuthService(
                _userManagerMock.Object,
                _signInManagerMock.Object,
                _roleManagerMock.Object,
                _tokenServiceMock.Object);
        }

        [Trait("Category", "Infrastructure")]
        [Fact]
        public async Task LoginValidateReturnsToke_WhenSuccess()
        {
            var username = "testuser";
            var password = "Strong@123";
            var user = new ApplicationUser { Id = "1", UserName = username, Email = "test@gmail.com", DisplayName = "Test User 1" };

            _userManagerMock.Setup(x => x.FindByNameAsync(username)).ReturnsAsync(user);

            _signInManagerMock.Setup(x => x.CheckPasswordSignInAsync(user, password, false))
                    .ReturnsAsync(SignInResult.Success);

            _tokenServiceMock.Setup(x => x.Generate(user.Id, user.UserName)).ReturnsAsync("SomeKindOfLooooongggggggTokenHere");

            var result = await _authService.LoginValidate(username, password);

            Assert.True(result.IsSuccess);
            Assert.Equal("SomeKindOfLooooongggggggTokenHere", result.Value);

            _userManagerMock.Verify(x => x.FindByNameAsync(username), Times.Once);
            _signInManagerMock.Verify(x => x.CheckPasswordSignInAsync(user, password, false), Times.Once);
            _tokenServiceMock.Verify(x => x.Generate(user.Id, user.UserName), Times.Once);
        }

        [Trait("Category", "Infrastructure")]
        [Fact]
        public async Task LoginUserNotFound_ReturnsFailure()
        {
            string user = "nonexistentuser";
            string password = "Strong@123";

            _userManagerMock.Setup(x => x.FindByNameAsync(user)).ReturnsAsync((ApplicationUser?)null);

            var result = await _authService.LoginValidate(user, password);

            Assert.False(result.IsSuccess);
            Assert.True(result.IsFailed);
            Assert.Contains("Username does not exist", result.Errors[0].Message);

            _userManagerMock.Verify(x => x.FindByNameAsync(user), Times.Once);
            _signInManagerMock.Verify(x => x.CheckPasswordSignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Never);
            _tokenServiceMock.Verify(x => x.Generate(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Trait("Category", "Infrastructure")]
        [Fact]
        public async Task LoginWithInvalidPassword_ReturnsFailure()
        {
            string username = "User1";
            string password = "wrongpasswprd";

            var user = new ApplicationUser { Id = "1", UserName = username };

            _userManagerMock.Setup(x => x.FindByNameAsync(username))
                .ReturnsAsync(user);
            _signInManagerMock.Setup(x => x.CheckPasswordSignInAsync(user, password, false))
                .ReturnsAsync(SignInResult.Failed);

            var result = await _authService.LoginValidate(username, password);

            Assert.False(result.IsSuccess);
            Assert.True(result.IsFailed);
            Assert.Contains("Incorrect Password", result.Errors[0].Message);

            _userManagerMock.Verify(x => x.FindByNameAsync(username), Times.Once);
            _signInManagerMock.Verify(x => x.CheckPasswordSignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Once);
            _tokenServiceMock.Verify(x => x.Generate(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
    }

}
