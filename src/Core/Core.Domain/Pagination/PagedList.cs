namespace Core.Domain.Pagination;
public class PagedList<T>
{
    public IReadOnlyCollection<T> Data { get; }
    public int TotalCount { get; }
    public int PageNumber { get; }
    public int PageSize { get; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasNextPage => PageNumber < TotalPages;
    public bool HasPreviousPage => PageNumber > 1;
    public PagedList(IEnumerable<T> items, int totalCount, int pageNumber, int pageSize)
    {
        Data = items.ToList().AsReadOnly();
        TotalCount = totalCount;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }
    public static PagedList<T> Empty(int pageNumber = 1, int pageSize = 10)
    {
        return new PagedList<T>(Enumerable.Empty<T>(), 0, pageNumber, pageSize);
    }
}