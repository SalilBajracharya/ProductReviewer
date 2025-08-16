using Microsoft.EntityFrameworkCore;
using ProductReviewer.Infrastructure.Data;

namespace ProductReviewer.Api.Extensions
{
    public static class RegisterServicesExtension
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<ApplicationDbContext>(opt =>
                opt.UseSqlServer(config.GetConnectionString("DefaultConnection"))
            );
            return services;
        }
    }
}
    