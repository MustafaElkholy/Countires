using Countries.Application.Validators.Countries;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;


namespace Countries.Application.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblyContaining<CreateCountryDtoValidator>();

            return services;
        }
    }
}
