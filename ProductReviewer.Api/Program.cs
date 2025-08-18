using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProductReviewer.Api.Extensions;
using ProductReviewer.Api.Middleware;
using ProductReviewer.Application;
using ProductReviewer.Infrastructure.Data;
using ProductReviewer.Infrastructure.Data.Identity;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(opt => {
        opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

RegisterIdentityExtension.RegisterIdentity(builder.Services);
RegisterAuthExtension.RegisterAuth(builder.Services, builder.Configuration);
RegisterSwaggerExtension.RegisterSwagger(builder.Services);
RegisterServicesExtension.RegisterServices(builder.Services, builder.Configuration);

RegisterApplicationServices.RegisterServices(builder.Services); //Application layer services

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{ 
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Product Reviewer API V1");
    c.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var db = services.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();

    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    await DbInitializer.SeedAsync(db, userManager, roleManager);
}


await app.RunAsync();
