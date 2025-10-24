namespace Core.Domain.Pagination;
public class PagedRequest
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; }
    public string? SortDirection { get; set; } = "asc";
    public bool IsValid()
    {
        if (PageNumber < 1)
            return false;
        if (PageSize < 1 || PageSize > 100)
            return false;
        if (!string.IsNullOrEmpty(SortDirection) &&
            SortDirection != "asc" && SortDirection != "desc")
            return false;
        return true;
    }
    public int GetSkipCount() => (PageNumber - 1) * PageSize;
}