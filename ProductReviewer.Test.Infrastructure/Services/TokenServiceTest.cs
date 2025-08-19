using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using ProductReviewer.Infrastructure.Data.Identity;
using ProductReviewer.Infrastructure.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ProductReviewer.Test.Infrastructure.Services
{
    public class TokenServiceTest
    {
        [Trait("Category", "Infrastructure")]
        [Fact]
        public async Task Generate_ShouldReturnToken_WithExpectedClaims()
        {
            var userid = "user-123";
            var userName = "testuser";

            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(c => c["Jwt:Key"]).Returns("THIS_IS_A_SECRET_KEY_FOR_JWT_TEST123");
            mockConfig.Setup(c => c["Jwt:Issuer"]).Returns("test_issuer");
            mockConfig.Setup(c => c["Jwt:Audience"]).Returns("test_audience");

            var mockUserStore = new Mock<IUserStore<ApplicationUser>>();
            //var userManagerMock = new Mock<UserManager<ApplicationUser>>(
            //    mockUserStore.Object, null, null, null, null, null, null, null, null);

            var userManagerMock = new Mock<UserManager<ApplicationUser>>(
                            mockUserStore.Object,
                            Mock.Of<IOptions<IdentityOptions>>(),
                            Mock.Of<IPasswordHasher<ApplicationUser>>(),
                            new IUserValidator<ApplicationUser>[0],
                            new IPasswordValidator<ApplicationUser>[0],
                            Mock.Of<ILookupNormalizer>(),
                            new IdentityErrorDescriber(),
                            Mock.Of<IServiceProvider>(),
                            Mock.Of<ILogger<UserManager<ApplicationUser>>>()
                        );

            var testUser = new ApplicationUser { Id = userid, UserName = userName };

            userManagerMock.Setup(um => um.FindByNameAsync(userName))
                .ReturnsAsync(testUser);
            userManagerMock.Setup(um => um.GetRolesAsync(testUser))
                .ReturnsAsync(new List<string> {"User"});

            var service = new TokenService(mockConfig.Object, userManagerMock.Object);

            var token = await service.Generate(testUser.Id, testUser.UserName);

            token.Should().NotBeNullOrWhiteSpace();

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            jwtToken.Claims.Should().Contain(c => c.Type == ClaimTypes.NameIdentifier && c.Value == userid);
            jwtToken.Claims.Should().Contain(c => c.Type == ClaimTypes.Name && c.Value == userName);
            jwtToken.Claims.Should().Contain(c => c.Type == ClaimTypes.Role && c.Value == "User");

            jwtToken.Issuer.Should().Be("test_issuer");

            userManagerMock.Verify(x => x.FindByNameAsync(userName), Times.Once);
            userManagerMock.Verify(x => x.GetRolesAsync(testUser), Times.Once);
        }

    }
}
