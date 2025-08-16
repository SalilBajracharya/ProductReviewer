using MediatR;
using Microsoft.AspNetCore.Identity;
using ProductReviewer.Application.Common.Interface;
using ProductReviewer.Application.Products.Queries;
using ProductReviewer.Infrastructure.Data;
using ProductReviewer.Infrastructure.Data.Identity;
using ProductReviewer.Infrastructure.Services;

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

            services.AddMediatR(cfg =>
                    cfg.RegisterServicesFromAssembly(typeof(GetAllProductsQuery).Assembly));
            services.AddScoped<IProductService, ProductService>();
            return services;
        }
    }
}
