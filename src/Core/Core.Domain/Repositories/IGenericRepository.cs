using Core.Application.Abstractions.Pagination;
using Core.Domain.Specifications;

namespace Core.Domain.Repositories;

/// <summary>
/// IGenericRepository - Generic repository interface
/// 
/// Tüm aggregate'ler için ortak repository interface
/// (Person, Payment, Student, vb.)
/// 
/// Sorumlulukları:
/// - CRUD operations
/// - Collection-like interface
/// - Tracking vs. No-tracking queries
/// - Query filtering ve pagination
/// 
/// Not: Sadece abstraction sağlar
/// Concrete implementation Infrastructure layer'da yapılır
/// </summary>
public interface IGenericRepository<TAggregate> where TAggregate : AggregateRoot
{
    // ==================== READ OPERATIONS ====================

    /// <summary>
    /// ID'ye göre aggregate getir
    /// </summary>
    Task<TAggregate?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);
    /// <summary>
    /// Specification'a göre tekil aggregate getir
    /// 
    /// Örnek:
    /// var spec = new PersonByNationalIdSpecification(nationalId);
    /// var person = await _repo.GetAsync(spec);
    /// 
    /// Döner: Null if not found
    /// </summary>
    Task<TAggregate?> GetAsync(
        ISpecification<TAggregate> specification,
        CancellationToken cancellationToken = default);


    /// <summary>
    /// Tüm aggregates'leri getir (soft deleted hariç)
    /// </summary>
    Task<IEnumerable<TAggregate>> GetAllAsync(
        CancellationToken cancellationToken = default);


    /// <summary>
    /// Sadece PagedRequest ile paginated liste getir
    /// 
    /// Kullanım:
    /// var pagedRequest = new PagedRequest 
    /// { 
    ///     PageNumber = 1, 
    ///     PageSize = 20,
    ///     SortBy = "Name",
    ///     SortDirection = "asc"
    /// };
    /// var pagedList = await _repo.GetAllAsync(pagedRequest);
    /// 
    /// Not:
    /// - Default specification otomatik uygulanır (soft delete kontrol)
    /// - Spec'ler tarafından sıralama override edilebilir
    /// - PagedList<TAggregate> döner
    /// </summary>
    Task<PagedList<TAggregate>> GetAllAsync(
        PagedRequest pagedRequest,
        CancellationToken cancellationToken = default);


    /// <summary>
    /// Specification'a göre paginated liste getir
    /// 
    /// Kullanım:
    /// var spec = new PersonsByDepartmentSpecification(deptId);
    /// var pagedRequest = new PagedRequest { PageNumber = 1, PageSize = 20 };
    /// var pagedList = await _repo.GetAllAsync(spec, pagedRequest);
    /// 
    /// Not:
    /// - Specification'dan ordering ve criteria uygulanır
    /// - PagedRequest'ten pagination ve sorting parametreleri uygulanır
    /// - PagedList<TAggregate> döner
    /// </summary>
    Task<PagedList<TAggregate>> GetAllAsync(
        ISpecification<TAggregate> specification,
        PagedRequest pagedRequest,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sadece Specification'a göre liste getir (pagination yok - memory'ye yükle)
    /// 
    /// ⚠️ DİKKAT: Pagination göndermezseniz TÜM sonuçlar döner!
    /// Sadece küçük result set'ler için kullanın.
    /// 
    /// Kullanım:
    /// var spec = new PersonsByDepartmentSpecification(deptId);
    /// var persons = await _repo.GetAllAsync(spec);  // TÜNEN SONUÇLAR!
    /// 
    /// İYİ: Küçük veri setleri (<1000 record)
    /// KÖTÜ: Büyük veri setleri (>10000 record)
    /// 
    /// Return: PagedList with all items in page 1
    /// </summary>
    Task<PagedList<TAggregate>> GetAllAsync(
        ISpecification<TAggregate> specification,
        CancellationToken cancellationToken = default);


    /// <summary>
    /// Aggregate'in var olup olmadığını kontrol et
    /// </summary>
    Task<bool> ExistsAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Toplam aggregate sayısını getir
    /// </summary>
    Task<int> CountAsync(
        CancellationToken cancellationToken = default);

    // ==================== WRITE OPERATIONS ====================

    /// <summary>
    /// Aggregate ekle
    /// </summary>
    Task AddAsync(
        TAggregate aggregate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Birden fazla aggregate ekle
    /// </summary>
    Task AddRangeAsync(
        IEnumerable<TAggregate> aggregates,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Aggregate güncelle
    /// </summary>
    Task UpdateAsync(
        TAggregate aggregate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Aggregate sil (soft delete)
    /// </summary>
    Task DeleteAsync(
        TAggregate aggregate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Birden fazla aggregate sil
    /// </summary>
    Task DeleteRangeAsync(
        IEnumerable<TAggregate> aggregates,
        CancellationToken cancellationToken = default);
}