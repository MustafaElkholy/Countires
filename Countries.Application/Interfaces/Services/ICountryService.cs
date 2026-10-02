using Countries.Application.DTOs.Countries.Request;
using Countries.Application.DTOs.Countries.Response;
using Countries.Application.DTOs.Shared;

namespace Countries.Application.Interfaces.Services
{
    public interface ICountryService
    {
        Task<GetCountryDto> CreateAsync(CreateCountryDto request, CancellationToken cancellationToken = default);
        Task<GetCountryDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<APIResponsePagedResult<GetCountryDto>> GetPagedAsync(GetCountriesRequestDto request, CancellationToken cancellationToken = default);
        Task<GetCountryDto> UpdateAsync(int id, UpdateCountryDto request, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    }

}