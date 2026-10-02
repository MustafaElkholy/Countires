using Countries.Application.Interfaces.Services;
using Countries.Infrastructure.Data;
using Countries.Infrastructure.Seeder;
using Countries.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Countries.Infrastructure.Extensions
{
    public static class DependencyInjectionServices
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("CountiresDbConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Connection string 'CountiresDbConnection' is missing.");
            }
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
            services.AddScoped<ICountrySeeder, CountrySeeder>();

            services.AddScoped<ICountryService, CountryService>();
            services.AddScoped<ICityService, CityService>();

            return services;
        }
    }
}
