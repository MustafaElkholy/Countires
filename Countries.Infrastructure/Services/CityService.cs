using Countries.Application.DTOs.Cities.Request;
using Countries.Application.DTOs.Cities.Response;
using Countries.Application.DTOs.Shared;
using Countries.Application.Exceptions;
using Countries.Application.Interfaces.Services;
using Countries.Domain.Entities;
using Countries.Infrastructure.Data;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;

namespace Countries.Infrastructure.Services
{
    internal sealed class CityService(
     ApplicationDbContext context,
     IValidator<CreateCityDto> createValidator,
     IValidator<UpdateCityDto> updateValidator,
     IValidator<GetCitiesRequestDto> queryValidator,
     IValidator<GetCitiesByCountryRequestDto> byCountryValidator)
     : ICityService
    {
        public async Task<GetCityDto> CreateAsync(CreateCityDto request, CancellationToken cancellationToken = default)
        {
            await createValidator.ValidateAndThrowAsync(request, cancellationToken);

            var country = await GetRequiredCountryAsync(request.CountryId, cancellationToken);

            var name = request.Name.Trim();

            await CheckDuplicateAsync(name, country.Id, null, cancellationToken);

            var city = new City
            {
                Name = name,
                CountryId = country.Id,
                Country = country
            };

            context.Cities.Add(city);
            await context.SaveChangesAsync(cancellationToken);

            return Map(city);
        }

        public async Task<GetCityDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var city = await GetRequiredCityAsync(id, cancellationToken);

            return Map(city);
        }

        public async Task<APIResponsePagedResult<GetCityDto>> GetPagedAsync(GetCitiesRequestDto request, CancellationToken cancellationToken = default)
        {
            await queryValidator.ValidateAndThrowAsync(request, cancellationToken);

            var query = context.Cities.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                var name = request.Name.Trim();

                query = query.Where(city => city.Name.Contains(name));
            }

            if (request.CountryId.HasValue)
            {
                query = query.Where(city =>
                    city.CountryId == request.CountryId.Value);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderBy(city => city.Name)
                .ThenBy(city => city.Id)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(city => new GetCityDto
                {
                    Id = city.Id,
                    Name = city.Name,
                    CountryId = city.CountryId,
                    CountryName = city.Country.Name
                })
                .ToListAsync(cancellationToken);

            return new APIResponsePagedResult<GetCityDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public async Task<APIResponsePagedResult<GetCityDto>> GetByCountryIdAsync(GetCitiesByCountryRequestDto request, CancellationToken cancellationToken = default)
        {
            await byCountryValidator.ValidateAndThrowAsync(request, cancellationToken);

            var country = await GetRequiredCountryAsync(request.CountryId, cancellationToken);

            return await GetPagedAsync(new GetCitiesRequestDto
            {
                CountryId = request.CountryId,
                Name = request.CountryName ?? null,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            }, cancellationToken);
        }

        public async Task<GetCityDto> UpdateAsync(int id, UpdateCityDto request, CancellationToken cancellationToken = default)
        {
            await updateValidator.ValidateAndThrowAsync(request, cancellationToken);

            var city = await GetRequiredCityAsync(id, cancellationToken);

            var country = await GetRequiredCountryAsync(request.CountryId, cancellationToken);

            var name = request.Name.Trim();

            await CheckDuplicateAsync(name, country.Id, id, cancellationToken);

            city.Name = name;
            city.CountryId = country.Id;
            city.Country = country;

            await context.SaveChangesAsync(cancellationToken);

            return Map(city);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var city = await GetRequiredCityAsync(id, cancellationToken);

            city.IsDeleted = true;

            await context.SaveChangesAsync(cancellationToken);
        }

        private async Task<City> GetRequiredCityAsync(int id, CancellationToken cancellationToken)
        {
            if (id <= 0)
            {
                throw new ValidationException(new[]
                {
                new ValidationFailure(
                    "Id",
                    "City ID must be greater than zero.")
            });
            }

            return await context.Cities
                .Include(city => city.Country)
                .FirstOrDefaultAsync(
                    city => city.Id == id,
                    cancellationToken)
                ?? throw new NotFoundException("City not found.");
        }

        private async Task<Country> GetRequiredCountryAsync(int countryId, CancellationToken cancellationToken)
        {
            return await context.Countries.FirstOrDefaultAsync(country => country.Id == countryId, cancellationToken)
                ?? throw new NotFoundException("Country not found.");
        }

        private async Task CheckDuplicateAsync(string name, int countryId, int? excludeId, CancellationToken cancellationToken)
        {
            var normalizedName = name.ToUpperInvariant();

            var exists = await context.Cities
                .AnyAsync(city =>
                    city.CountryId == countryId
                    && (!excludeId.HasValue || city.Id != excludeId.Value) && city.Name.ToUpper() == normalizedName, cancellationToken);

            if (exists)
            {
                throw new ConflictException(
                    "A city with this name already exists in this country.");
            }
        }

        private static GetCityDto Map(City city)
        {
            return new GetCityDto
            {
                Id = city.Id,
                Name = city.Name,
                CountryId = city.CountryId,
                CountryName = city.Country.Name
            };
        }
    }
}
