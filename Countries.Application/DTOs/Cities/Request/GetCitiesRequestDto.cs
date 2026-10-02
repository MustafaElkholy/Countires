using Countries.Application.DTOs.Shared;

namespace Countries.Application.DTOs.Cities.Request
{
    public class GetCitiesRequestDto : PaginationQueryDto
    {
        public string? Name { get; set; }
        public int? CountryId { get; set; }

    }
}
