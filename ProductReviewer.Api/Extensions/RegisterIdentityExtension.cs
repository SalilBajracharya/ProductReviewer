using Microsoft.AspNetCore.Identity;
using ProductReviewer.Infrastructure.Data;
using ProductReviewer.Infrastructure.Data.Identity;

namespace ProductReviewer.Api.Extensions
{
    public static class RegisterIdentityExtension
    {
        public static IServiceCollection RegisterIdentity(this IServiceCollection services)
        {
            services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
            })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            return services;
        }
    }
}
