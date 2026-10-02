using Countries.Application.DTOs.Cities.Request;
using Countries.Application.DTOs.Cities.Response;
using Countries.Application.DTOs.Shared;

namespace Countries.Application.Interfaces.Services
{
    public interface ICityService
    {
        Task<GetCityDto> CreateAsync(CreateCityDto request, CancellationToken cancellationToken = default);
        Task<GetCityDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<APIResponsePagedResult<GetCityDto>> GetPagedAsync(GetCitiesRequestDto request, CancellationToken cancellationToken = default);
        Task<APIResponsePagedResult<GetCityDto>> GetByCountryIdAsync(GetCitiesByCountryRequestDto request, CancellationToken cancellationToken = default);
        Task<GetCityDto> UpdateAsync(int id, UpdateCityDto request, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
