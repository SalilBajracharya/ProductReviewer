using Microsoft.AspNetCore.Identity;
using ProductReviewer.Api.Extensions;
using ProductReviewer.Infrastructure.Data;
using ProductReviewer.Infrastructure.Data.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
RegisterSwaggerExtension.RegisterSwagger(builder.Services);
RegisterServicesExtension.RegisterServices(builder.Services, builder.Configuration);
RegisterIdentityExtension.RegisterIdentity(builder.Services);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{ 
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Product Reviewer API V1");
    c.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var db = services.GetRequiredService<ApplicationDbContext>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    await DbInitializer.SeedAsync(db, userManager);
}

await app.RunAsync();
