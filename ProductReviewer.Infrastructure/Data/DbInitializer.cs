using Microsoft.AspNetCore.Identity;
using ProductReviewer.Domain.Constants;
using ProductReviewer.Domain.Entities;
using ProductReviewer.Infrastructure.Data.Identity;

namespace ProductReviewer.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(ApplicationDbContext ctx, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (!ctx.Products.Any())
            {
                var p1 = new Product
                {
                    Name = "Laptop",
                    SKU = "LP1001",
                    ProductType = "Electronics",
                    Description = "HP Laptop,3rd Generation."
                };

                var p2 = new Product
                {
                    Name = "DSLR Camera",
                    SKU = "CAM2B01",
                    ProductType = "Electronics",
                    Description = "Cannon Camera."
                };

                ctx.Products.AddRange(p1, p2);
                await ctx.SaveChangesAsync();
            }

            if (!roleManager.Roles.Any())
            {
                var roles = new List<IdentityRole>
                {
                    new IdentityRole { Name = UserRoles.Admin },
                    new IdentityRole { Name = UserRoles.User },
                    new IdentityRole { Name = UserRoles.Reviewer }
                };

                foreach (var role in roles)
                {
                    var roleExists = await roleManager.RoleExistsAsync(role.Name!);

                    if (!roleExists)
                    {
                        await roleManager.CreateAsync(role);
                    }
                }
            }

            if (!userManager.Users.Any())
            {
                var user = new ApplicationUser { UserName = "Super", DisplayName = "Super User", Email = "test@gmail.com" };
                await userManager.CreateAsync(user, "MyPassword1!");

                var roleExists = await roleManager.RoleExistsAsync(UserRoles.Admin);
                await userManager.AddToRoleAsync(user, UserRoles.Admin);
            }
        }
    }
}
