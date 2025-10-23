namespace Core.Domain.Pagination;

/// <summary>
/// PagedList<T> - Sayfalandırılmış sonuç
/// 
/// Sorumluluğu:
/// - Query handler'lardan gelen sayfalandırılmış verileri wrap etme
/// 
/// Kullanım:
/// var response = new PagedList<PersonResponse>(
///     items: persons,
///     totalCount: 150,
///     pageNumber: 1,
///     pageSize: 10);
/// 
/// return Result<PagedList<PersonResponse>>.Success(response);
/// 
/// Example JSON response:
/// {
///   "data": [ ... ],
///   "totalCount": 150,
///   "pageNumber": 1,
///   "pageSize": 10,
///   "totalPages": 15,
///   "hasNextPage": true,
///   "hasPreviousPage": false
/// }
/// </summary>
public class PagedList<T>
{
    /// <summary>
    /// Sayfadaki öğeler
    /// </summary>
    public IReadOnlyCollection<T> Data { get; }

    /// <summary>
    /// Toplam öğe sayısı
    /// </summary>
    public int TotalCount { get; }

    /// <summary>
    /// Sayfa numarası
    /// </summary>
    public int PageNumber { get; }

    /// <summary>
    /// Sayfa boyutu
    /// </summary>
    public int PageSize { get; }

    /// <summary>
    /// Toplam sayfa sayısı
    /// </summary>
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    /// <summary>
    /// Sonraki sayfa var mı?
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;

    /// <summary>
    /// Önceki sayfa var mı?
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    public PagedList(IEnumerable<T> items, int totalCount, int pageNumber, int pageSize)
    {
        Data = items.ToList().AsReadOnly();
        TotalCount = totalCount;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }

    /// <summary>
    /// Boş PagedList oluştur
    /// </summary>
    public static PagedList<T> Empty(int pageNumber = 1, int pageSize = 10)
    {
        return new PagedList<T>(Enumerable.Empty<T>(), 0, pageNumber, pageSize);
    }
}