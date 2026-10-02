namespace Countries.Application.DTOs.Shared
{
    public class PaginationQueryDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
