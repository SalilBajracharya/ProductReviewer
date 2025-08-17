using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ProductReviewer.Application.Common.Behaviours;
using System.Reflection;

namespace ProductReviewer.Application
{
    public static class RegisterApplicationServices
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();

            services.AddValidatorsFromAssembly(assembly);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            return services;
        }
    }
}
