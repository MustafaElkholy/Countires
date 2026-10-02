using Countries.Application.DTOs.Countries.Request;
using Countries.Application.DTOs.Countries.Response;
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
    internal sealed class CountryService(
        ApplicationDbContext context,
        IValidator<CreateCountryDto> createValidator,
        IValidator<UpdateCountryDto> updateValidator,
        IValidator<GetCountriesRequestDto> queryValidator)
        : ICountryService
    {
        public async Task<GetCountryDto> CreateAsync(CreateCountryDto request, CancellationToken cancellationToken = default)
        {
            await createValidator.ValidateAndThrowAsync(request, cancellationToken);

            var name = request.Name.Trim();
            var code = request.Code.ToUpperInvariant();

            await CheckDuplicatesAsync(name, code, null, cancellationToken);

            var country = new Country
            {
                Name = name,
                Code = code
            };

            context.Countries.Add(country);
            await context.SaveChangesAsync(cancellationToken);

            return Map(country);
        }

        public async Task<GetCountryDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var country = await GetRequiredAsync(id, cancellationToken);

            return Map(country);
        }

        public async Task<APIResponsePagedResult<GetCountryDto>> GetPagedAsync(GetCountriesRequestDto request, CancellationToken cancellationToken = default)
        {
            await queryValidator.ValidateAndThrowAsync(request, cancellationToken);

            var query = context.Countries.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                var name = request.Name.Trim();

                query = query.Where(country =>
                    country.Name.Contains(name));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderBy(country => country.Name)
                .ThenBy(country => country.Id)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(country => new GetCountryDto
                {
                    Id = country.Id,
                    Name = country.Name,
                    Code = country.Code
                })
                .ToListAsync(cancellationToken);

            return new APIResponsePagedResult<GetCountryDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public async Task<GetCountryDto> UpdateAsync(int id, UpdateCountryDto request, CancellationToken cancellationToken = default)
        {
            await updateValidator.ValidateAndThrowAsync(request, cancellationToken);

            var country = await GetRequiredAsync(id, cancellationToken);

            var name = request.Name.Trim();
            var code = request.Code.ToUpperInvariant();

            await CheckDuplicatesAsync(name, code, id, cancellationToken);

            country.Name = name;
            country.Code = code;

            await context.SaveChangesAsync(cancellationToken);

            return Map(country);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var country = await GetRequiredAsync(id, cancellationToken);

            var hasCities = await context.Cities.AnyAsync(city => city.CountryId == id, cancellationToken);

            if (hasCities)
            {
                throw new ConflictException(
                    "Cannot delete this country because it has active cities.");
            }

            country.IsDeleted = true;

            await context.SaveChangesAsync(cancellationToken);
        }

        private async Task<Country> GetRequiredAsync(int id, CancellationToken cancellationToken)
        {
            if (id <= 0)
            {
                throw new ValidationException(new[]
                {
                new ValidationFailure(
                    "Id",
                    "Country ID must be greater than zero.")
            });
            }

            return await context.Countries.FirstOrDefaultAsync(
                country => country.Id == id,
                cancellationToken)
                ?? throw new NotFoundException("Country not found.");
        }

        private async Task CheckDuplicatesAsync(string name, string code, int? excludeId, CancellationToken cancellationToken)
        {
            var normalizedName = name.ToUpperInvariant();

            var nameExists = await context.Countries.AnyAsync(
                country => (!excludeId.HasValue || country.Id != excludeId.Value) && country.Name.ToUpper() == normalizedName, cancellationToken);

            if (nameExists)
            {
                throw new ConflictException(
                    "A country with this name already exists.");
            }

            var codeExists = await context.Countries
                .AnyAsync(country => (!excludeId.HasValue || country.Id != excludeId.Value) && country.Code.ToUpper() == code, cancellationToken);

            if (codeExists)
            {
                throw new ConflictException(
                    "A country with this code already exists.");
            }
        }

        private static GetCountryDto Map(Country country)
        {
            return new GetCountryDto
            {
                Id = country.Id,
                Name = country.Name,
                Code = country.Code
            };
        }
    }
}
