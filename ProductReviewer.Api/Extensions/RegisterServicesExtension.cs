using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductReviewer.Application.Common.Interface;
using ProductReviewer.Application.Segregation.Products.Queries;
using ProductReviewer.Infrastructure.Data;
using ProductReviewer.Infrastructure.Services;

namespace ProductReviewer.Api.Extensions
{
    public static class RegisterServicesExtension
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<ApplicationDbContext>(opt =>
                opt.UseSqlServer(config.GetConnectionString("DefaultConnection"))
            );

            services.AddMediatR(cfg =>
                    cfg.RegisterServicesFromAssembly(typeof(GetAllProductsQuery).Assembly));

            services.AddScoped<ICurrentUser, CurrentUserService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddSingleton<ICsvGenerator, CsvGeneratorService>();
            return services;
        }
    }
}
    