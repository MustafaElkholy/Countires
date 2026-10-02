using Countries.Application.DTOs.Shared;

namespace Countries.Application.DTOs.Cities.Request
{
    public class GetCitiesByCountryRequestDto : PaginationQueryDto
    {
        public int CountryId { get; set; }
        public string? CountryName { get; set; } = null;
    }
}
