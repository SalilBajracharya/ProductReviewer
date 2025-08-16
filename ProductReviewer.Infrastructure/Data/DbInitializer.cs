using Microsoft.AspNetCore.Identity;
using ProductReviewer.Domain.Entities;
using ProductReviewer.Infrastructure.Data.Identity;

namespace ProductReviewer.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(ApplicationDbContext ctx, UserManager<ApplicationUser> userManager)
        { 
            if(!ctx.Products.Any())
            {
                var p1 = new Product 
                { Name = "Laptop", SKU = "LP1001", ProductType = "Electronics", 
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

            if(!userManager.Users.Any())
            {
                var user = new ApplicationUser { UserName = "Super", DisplayName = "Super User", Email = "test@gmail.com" };
                await userManager.CreateAsync(user, "MyPassword1!");
            }
        }
    }
}
