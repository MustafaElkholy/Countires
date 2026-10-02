using Countries.API.Middleware;
using Countries.Application.Extensions;
using Countries.Infrastructure.Data;
using Countries.Infrastructure.Extensions;
using Countries.Infrastructure.Seeder;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();


builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddServices();
builder.Services.AddScoped<ErrorHandlingMiddleware>();

var app = builder.Build();

app.UseExceptionHandler();
app.UseMiddleware<ErrorHandlingMiddleware>();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    await using var scope = app.Services.CreateAsyncScope();

    var context = scope.ServiceProvider
        .GetRequiredService<ApplicationDbContext>();

    await context.Database.MigrateAsync();

    var seeder = scope.ServiceProvider
        .GetRequiredService<ICountrySeeder>();

    await seeder.Seed();

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
