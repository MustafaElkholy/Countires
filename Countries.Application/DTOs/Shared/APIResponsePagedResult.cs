namespace Countries.Application.DTOs.Shared
{
    public class APIResponsePagedResult<T>
    {
        public List<T> Items { get; init; } = new();
        public int TotalCount { get; init; }
        public int PageNumber { get; init; }
        public int PageSize { get; init; }

        public int TotalPages => PageSize > 0
            ? (int)Math.Ceiling(TotalCount / (double)PageSize)
            : 0;

        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }
}
