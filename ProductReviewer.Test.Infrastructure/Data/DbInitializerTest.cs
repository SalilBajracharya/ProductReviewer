using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using ProductReviewer.Domain.Constants;
using ProductReviewer.Domain.Entities;
using ProductReviewer.Infrastructure.Data;
using ProductReviewer.Infrastructure.Data.Identity;

namespace ProductReviewer.Test.Infrastructure.Data
{
    public class DbInitializerTest
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly Mock<UserManager<ApplicationUser>> _userManager;
        private readonly Mock<RoleManager<IdentityRole>> _roleManager;

        public DbInitializerTest()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new ApplicationDbContext(options);

            var userStore = new Mock<IUserStore<ApplicationUser>>();
            //_userManager = new Mock<UserManager<ApplicationUser>>(
            //    userStore.Object, null, null, null, null, null, null, null, null);
            _userManager = new Mock<UserManager<ApplicationUser>>(
                            userStore.Object,
                            Mock.Of<IOptions<IdentityOptions>>(),
                            Mock.Of<IPasswordHasher<ApplicationUser>>(),
                            new IUserValidator<ApplicationUser>[0],
                            new IPasswordValidator<ApplicationUser>[0],
                            Mock.Of<ILookupNormalizer>(),
                            new IdentityErrorDescriber(),
                            Mock.Of<IServiceProvider>(),
                            Mock.Of<ILogger<UserManager<ApplicationUser>>>()
                        );

            var roleStore = new Mock<IRoleStore<IdentityRole>>();
            //_roleManager = new Mock<RoleManager<IdentityRole>>(
            //    roleStore.Object, null, null, null, null);

            _roleManager = new Mock<RoleManager<IdentityRole>>(
                                roleStore.Object,
                                new IRoleValidator<IdentityRole>[0],
                                Mock.Of<ILookupNormalizer>(),
                                new IdentityErrorDescriber(),
                                Mock.Of<ILogger<RoleManager<IdentityRole>>>()
);
        }

        [Trait("Category", "Infrastructure")]
        [Fact]
        public async Task SeedAsync_AddsDefaultProduct_WhenDBEmpty()
        {
            await DbInitializer.SeedAsync(_dbContext, _userManager.Object, _roleManager.Object);
            var product = _dbContext.Products.ToList();

            Assert.Equal(2, product.Count);
            Assert.Contains(product, p => p.Name == "Laptop" && p.SKU == "LP1001");
            Assert.Contains(product, p => p.Name == "DSLR Camera" && p.SKU == "CAM2B01");
        }


        [Trait("Category", "Infrastructure")]
        [Fact]
        public async Task SeedAsync_SkipsProductSeeding_WhenProductsExist()
        {
            _dbContext.Products.Add(new Product { Name = "Existing Product", Description = "Test Description", SKU = "EX123", ProductType = "Electronics" });
            await _dbContext.SaveChangesAsync();

            await DbInitializer.SeedAsync(_dbContext, _userManager.Object, _roleManager.Object);

            var products = _dbContext.Products.ToList();
            Assert.Single(products);
        }

        [Trait("Category", "Infrastructure")]
        [Fact]
        public async Task SeedAsync_AddsDefaultRoles_WhenRolesEmpty()
        {
            _roleManager.Setup(r => r.Roles).Returns(new List<IdentityRole>().AsQueryable());
            _roleManager.Setup(r => r.RoleExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(false);
            _roleManager.Setup(r => r.CreateAsync(It.IsAny<IdentityRole>()))
                .ReturnsAsync(IdentityResult.Success);

            await DbInitializer.SeedAsync(_dbContext, _userManager.Object, _roleManager.Object);

            _roleManager.Verify(r => r.CreateAsync(It.IsAny<IdentityRole>()), Times.Exactly(3));
        }

        [Trait("Category", "Infrastructure")]
        [Fact]
        public async Task SeedAsync_SkipsRoleCreation_WhenRolesEmpty()
        {
            _roleManager.Setup(r => r.Roles).Returns(new List<IdentityRole> { new IdentityRole(UserRoles.Admin) }.AsQueryable());
            _roleManager.Setup(r => r.RoleExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            await DbInitializer.SeedAsync(_dbContext, _userManager.Object, _roleManager.Object);

            _roleManager.Verify(r => r.CreateAsync(It.IsAny<IdentityRole>()), Times.Never);
        }

        [Trait("Category", "Infrastructure")]
        [Fact]
        public async Task SeedAsync_CreateDefaultAdminUser_WhenUsersEmpty()
        {
            _userManager.Setup(u => u.Users).Returns(new List<ApplicationUser>().AsQueryable());
            _userManager.Setup(u => u.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            _roleManager.Setup(r => r.RoleExistsAsync(UserRoles.Admin)).ReturnsAsync(true);
            _userManager.Setup(u => u.AddToRoleAsync(It.IsAny<ApplicationUser>(), UserRoles.Admin))
                .ReturnsAsync(IdentityResult.Success);

            await DbInitializer.SeedAsync(_dbContext, _userManager.Object, _roleManager.Object);

            _userManager.Verify(u => u.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once);
            _userManager.Verify(u => u.AddToRoleAsync(It.IsAny<ApplicationUser>(), UserRoles.Admin), Times.Once);
        }

        [Trait("Category", "Infrastructure")]
        [Fact]
        public async Task SeedAsync_SkipUserCreation_WhenUserExists()
        {
            _userManager.Setup(u => u.Users).Returns(new List<ApplicationUser> { new ApplicationUser() }.AsQueryable());

            await DbInitializer.SeedAsync(_dbContext, _userManager.Object, _roleManager.Object);

            _userManager.Verify(u => u.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
            _userManager.Verify(u => u.AddToRoleAsync(It.IsAny<ApplicationUser>(), UserRoles.Admin), Times.Never);
        }

    }
}
