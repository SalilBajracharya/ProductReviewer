using Microsoft.EntityFrameworkCore.Internal;
using ProductReviewer.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
RegisterServicesExtension.RegisterServices(builder.Services, builder.Configuration);
RegisterIdentityExtension.RegisterIdentity(builder.Services);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
