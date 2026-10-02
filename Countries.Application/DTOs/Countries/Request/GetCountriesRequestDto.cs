using Countries.Application.DTOs.Shared;

namespace Countries.Application.DTOs.Countries.Request
{
    public class GetCountriesRequestDto : PaginationQueryDto
    {
        public string? Name { get; set; }
    }
}
