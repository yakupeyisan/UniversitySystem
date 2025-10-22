namespace Core.Application.Abstractions.Pagination;

/// <summary>
/// PagedRequest - Pagination parameters
/// 
/// Sorumluluğu:
/// - Sayfalama parametrelerini standartlaştırma
/// - Query'de kullanılmak üzere
/// 
/// Kullanım:
/// var query = new GetPersonsQuery(
///     new PagedRequest { PageNumber = 1, PageSize = 10, SortBy = "Name", SortDirection = "asc" });
/// var result = await mediator.Send(query);
/// 
/// Properties:
/// - PageNumber: 1'den başlayan sayfa numarası
/// - PageSize: Her sayfada kaç kayıt? (default 10)
/// - SortBy: Sıralama yapılacak field adı
/// - SortDirection: "asc" veya "desc"
/// 
/// Example:
/// {
///   "pageNumber": 1,
///   "pageSize": 20,
///   "sortBy": "Name",
///   "sortDirection": "asc"
/// }
/// </summary>
public class PagedRequest
{
    /// <summary>
    /// Sayfa numarası (1-based)
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Sayfa boyutu
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Sıralanacak field adı
    /// </summary>
    public string? SortBy { get; set; }

    /// <summary>
    /// Sıralama yönü: "asc" veya "desc"
    /// </summary>
    public string? SortDirection { get; set; } = "asc";

    /// <summary>
    /// Validation et
    /// </summary>
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

    /// <summary>
    /// Skip count'u hesapla
    /// </summary>
    public int GetSkipCount() => (PageNumber - 1) * PageSize;
}