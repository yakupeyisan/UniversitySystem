# 🔍 Specification_Guide.md - Advanced Filtering, Sorting & Pagination

## 📖 Specification Pattern Nedir?

Specification pattern, **repository method explosion'ını** önlemek için kullanılan bir design pattern'dır. Queries'ı **encapsulate** eder.

### ❌ Spec Pattern OLMADAN

```csharp
public interface IPersonRepository : IRepository<Person>
{
    Task<List<Person>> GetActiveAsync();
    Task<List<Person>> GetStudentsAsync();
    Task<List<Person>> GetStaffsAsync();
    Task<List<Person>> GetPersonsByAgeAsync(int age);
    Task<List<Person>> GetPersonsByEmailAsync(string email);
    Task<List<Person>> GetActiveStudentsWithEnrollmentsAsync();
    Task<List<Person>> GetStaffsByDepartmentAsync(string dept);
    Task<List<Person>> GetPersonsByCreatedDateRangeAsync(DateTime from, DateTime to);
    // ... 50+ methods
}
```

### ✅ Spec Pattern İLE

```csharp
// Generic interface - HEP AYNI
public interface IRepository<TEntity>
{
    Task<PagedList<TEntity>> GetAllAsync(ISpecification<TEntity> spec, PagedRequest request, CancellationToken ct);
    Task<TEntity?> GetAsync(ISpecification<TEntity> spec, CancellationToken ct);
    // ... sadece 8-10 generic method
}

// Specification'lar (esnek, modüler, reusable)
var spec = new GetActiveStudentsSpecification();
var students = await _repository.GetAllAsync(spec, new PagedRequest(1, 10), ct);

var spec2 = new GetStaffsByDepartmentSpecification("IT");
var staffs = await _repository.GetAllAsync(spec2, new PagedRequest(1, 20), ct);
```

---

## 🎯 SPECIFICATION BASE CLASSES

### BaseSpecification<T> (Basit, çok modülde kullanılabilir)

```csharp
public abstract class BaseSpecification<T> : ISpecification<T> where T : class
{
    // Constructor
    protected BaseSpecification()                                    // No criteria
    protected BaseSpecification(Expression<Func<T, bool>> criteria) // With criteria

    // Properties
    public Expression<Func<T, bool>>? Criteria { get; }           // WHERE
    public List<Expression<Func<T, object>>> Includes { get; }    // Eager loading
    public List<string> IncludeStrings { get; }                   // String-based includes
    public List<(...OrderExpression..., bool ...Descending)> OrderBys { get; }  // ORDER BY
    public int? Take { get; }                                      // Paging: take
    public int? Skip { get; }                                      // Paging: skip
    public bool IsPagingEnabled { get; }
    public bool IsSplitQuery { get; }                             // EF Core optimization

    // Methods
    protected void AddCriteria(Expression<Func<T, bool>> criteria) // AND multiple criteria
    protected void AddInclude(Expression<Func<T, object>> expr)
    protected void AddIncludeString(string path)                  // e.g. "Student.Enrollments"
    protected void AddOrderBy<TKey>(Expression<Func<T, TKey>> expr)
    protected void AddOrderByDescending<TKey>(Expression<Func<T, TKey>> expr)
    protected void AddOrderBy<TKey>(Expression<Func<T, TKey>> expr, bool desc)
    protected void ApplyPaging(int skip, int take)               // Manual paging
    protected void RemovePaging()
    protected void UseSplitQuery()                               // EF optimization
}
```

### BaseFilteredSpecification<T> (Advanced - filtering + paging)

```csharp
public abstract class BaseFilteredSpecification<T> : BaseSpecification<T>
    where T : class
{
    // Constructors - 5 overload
    protected BaseFilteredSpecification()                                          // No criteria
    protected BaseFilteredSpecification(Expression<Func<T, bool>> filter)         // Manual filter
    protected BaseFilteredSpecification(string? filterString, IFilterParser<T> fp) // Dynamic filter
    protected BaseFilteredSpecification(
        string? filterString, 
        IFilterParser<T> fp, 
        int pageNumber, 
        int pageSize)                                                              // Dynamic + paging
    protected BaseFilteredSpecification(
        string? filterString,
        IFilterParser<T> fp,
        IFilterWhitelist whitelist,
        int pageNumber,
        int pageSize)                                                              // Dynamic + whitelist

    // Methods
    protected void ApplySoftDeleteFilter()                      // ISoftDelete support
    protected void AddIncludes(params Expression<Func<T, object?>>[] includes)    // Batch includes
    protected void ApplyOrderBy<TKey>(..., bool descending)
}
```

---

## 💡 SPECIFICATION OLUŞTURMA ÖRNEKLERI

### Örnek 1: Basit Specification (Criteria + Includes + Sorting)

```csharp
public class GetActiveStudentsSpecification : BaseFilteredSpecification<Person>
{
    public GetActiveStudentsSpecification()
    {
        // Step 1: Eager Loading
        AddInclude(p => p.Student);
        AddInclude(p => p.HealthRecord);
        
        // Step 2: Filtering (WHERE)
        AddCriteria(p => p.Student != null);
        AddCriteria(p => !p.IsDeleted);
        AddCriteria(p => !p.Student.IsDeleted);
        
        // Step 3: Soft Delete Check
        ApplySoftDeleteFilter();
        
        // Step 4: Sorting
        AddOrderBy(p => p.Student!.StudentNumber);
        
        // NOT paging yapılmıyor - Repository'de PagedRequest ile yapılacak
    }
}

// Kullanım
var spec = new GetActiveStudentsSpecification();
var pagedList = await _repository.GetAllAsync(
    spec,
    new PagedRequest(pageNumber: 1, pageSize: 10),
    cancellationToken);
```

### Örnek 2: Paging + Sorting ile Specification

```csharp
public class GetPersonsWithPagingSpecification : BaseFilteredSpecification<Person>
{
    public GetPersonsWithPagingSpecification(
        int pageNumber = 1,
        int pageSize = 20)
    {
        AddInclude(p => p.Student);
        AddCriteria(p => !p.IsDeleted);
        
        // SPEC'TE PAGING
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
        
        AddOrderByDescending(p => p.CreatedAt);
    }
}

// Kullanım - Repository'de PagedRequest ile paging yapılmayacak (spec'te yapıldı)
var spec = new GetPersonsWithPagingSpecification(pageNumber: 1, pageSize: 10);
var pagedList = await _repository.GetAllAsync(spec, new PagedRequest(), cancellationToken);
```

### Örnek 3: Dynamic Filtering (String-based)

```csharp
public class GetPersonsWithFiltersSpecification : BaseFilteredSpecification<Person>
{
    public GetPersonsWithFiltersSpecification(
        string? filterString,           // Format: "firstName=John;lastName=Doe"
        int pageNumber = 1,
        int pageSize = 20)
    {
        AddInclude(p => p.Student);
        AddInclude(p => p.HealthRecord);
        
        // Dynamic filtering - whitelist ile security
        if (!string.IsNullOrWhiteSpace(filterString))
        {
            var filterParser = new FilterParser<Person>(
                whitelist: new PersonFilterWhitelist());  // ← Allowed properties
            var filterExpression = filterParser.Parse(filterString);
            AddCriteria(filterExpression);
        }
        
        ApplySoftDeleteFilter();
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
        AddOrderBy(p => p.LastName);
    }
}

// Kullanım
var filterString = "firstName=John;personType=Student";  // URL query parameter'dan
var spec = new GetPersonsWithFiltersSpecification(filterString, 1, 10);
var result = await _repository.GetAllAsync(spec, new PagedRequest(), cancellationToken);
```

### Örnek 4: Nested Includes (Include String)

```csharp
public class GetStudentsWithAllDataSpecification : BaseFilteredSpecification<Person>
{
    public GetStudentsWithAllDataSpecification()
    {
        // Simple includes
        AddInclude(p => p.Student);
        AddInclude(p => p.HealthRecord);
        
        // Nested includes (string-based)
        AddIncludeString("Student.Enrollments");           // Person → Student → Enrollments
        AddIncludeString("Student.Enrollments.Course");    // Person → Student → Enrollments → Course
        
        // Soft delete + filtering
        AddCriteria(p => p.Student != null && !p.IsDeleted);
        ApplySoftDeleteFilter();
        
        // EF Core optimization
        UseSplitQuery();  // ← Multiple queries (N+1 prevention)
        
        // Sorting
        AddOrderBy(p => p.LastName);
        AddOrderBy(p => p.FirstName);
    }
}
```

---

## 🔐 FILTERING - Dynamic + Safe

### Step 1: Filter Whitelist Tanımla

```csharp
// PersonFilterWhitelist.cs - Hangi field'lar filtrelenebilir?
public class PersonFilterWhitelist : IFilterWhitelist
{
    private static readonly HashSet<string> AllowedProperties = new()
    {
        nameof(Person.FirstName),
        nameof(Person.LastName),
        nameof(Person.Email),
        nameof(Person.PersonType),
        nameof(Person.DateOfBirth),
        // NEVER include sensitive data!
        // nameof(Person.Password),        // ✗
        // nameof(Person.InternalNotes),   // ✗
    };

    public bool IsAllowed(string propertyName)
    {
        return AllowedProperties.Contains(propertyName);
    }

    public IReadOnlySet<string> GetAllowedProperties()
    {
        return AllowedProperties.AsReadOnly();
    }
}
```

### Step 2: Specification'da Kullan

```csharp
public class GetPersonsWithFiltersSpecification : BaseFilteredSpecification<Person>
{
    public GetPersonsWithFiltersSpecification(
        string? filterString,
        int pageNumber = 1,
        int pageSize = 20)
    {
        AddInclude(p => p.Student);
        
        // Whitelist ile güvenli filtering
        if (!string.IsNullOrWhiteSpace(filterString))
        {
            var filterParser = new FilterParser<Person>(
                whitelist: new PersonFilterWhitelist());
            var filterExpression = filterParser.Parse(filterString);  // ← Parse & validate
            AddCriteria(filterExpression);
        }
        
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
    }
}
```

### Step 3: API'de Kullan

```csharp
[HttpGet]
public async Task<IActionResult> GetPersons(
    [FromQuery] string? filterString,    // "firstName=John;email=@gmail.com"
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10,
    CancellationToken cancellationToken = default)
{
    var spec = new GetPersonsWithFiltersSpecification(filterString, pageNumber, pageSize);
    var result = await _repository.GetAllAsync(spec, new PagedRequest(pageNumber, pageSize), cancellationToken);
    // ...
}

// URL Example:
// GET /api/persons?filterString=firstName=John;personType=Student&pageNumber=1&pageSize=10
```

---

## 📊 SORTING - Çoklu Fields

```csharp
public class GetStudentsSortedSpecification : BaseFilteredSpecification<Person>
{
    public GetStudentsSortedSpecification(bool orderByDepartment = false)
    {
        AddInclude(p => p.Student);
        AddCriteria(p => p.Student != null && !p.IsDeleted);
        
        // Conditional sorting
        if (orderByDepartment)
        {
            AddOrderBy(p => p.Student!.Department);
            AddOrderBy(p => p.LastName);
        }
        else
        {
            AddOrderByDescending(p => p.CreatedAt);
            AddOrderBy(p => p.FirstName);
        }
    }
}
```

**Multiple OrderBy Logic:**
```csharp
// EF Core ilk AddOrderBy → ThenBy yapacak
AddOrderBy(p => p.LastName);      // ORDER BY LastName
AddOrderBy(p => p.FirstName);     // THEN BY FirstName
AddOrderByDescending(p => p.CreatedAt);  // THEN BY CreatedAt DESC
```

---

## 📄 PAGINATION - PagedList + PagedRequest

### PagedList<T> (Response)

```csharp
public class PagedList<T>
{
    public IReadOnlyCollection<T> Data { get; }
    public int TotalCount { get; }
    public int PageNumber { get; }
    public int PageSize { get; }
    public int TotalPages { get; }
    public bool HasNextPage { get; }
    public bool HasPreviousPage { get; }
    
    public static PagedList<T> Empty(int pageNumber = 1, int pageSize = 10)
}
```

### PagedRequest (Query Parameter)

```csharp
public class PagedRequest
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; }
    public string? SortDirection { get; set; } = "asc";
    
    public bool IsValid()
    {
        return PageNumber >= 1 && PageSize >= 1 && PageSize <= 100;
    }
    
    public int GetSkipCount() => (PageNumber - 1) * PageSize;
}
```

### Paging Senaryoları

#### Senaryo 1: Spec'te Paging YOKSA → Repository'de PagedRequest ile

```csharp
public class GetStudentsSpecification : BaseFilteredSpecification<Person>
{
    public GetStudentsSpecification()
    {
        AddInclude(p => p.Student);
        AddCriteria(p => p.Student != null && !p.IsDeleted);
        // ApplyPaging() YAPILMIYOR
    }
}

// Repository'de PagedRequest ile paging yapılacak
var spec = new GetStudentsSpecification();
var pagedList = await _repository.GetAllAsync(
    spec,
    new PagedRequest(pageNumber: 2, pageSize: 15),  // ← Paging burada
    cancellationToken);
```

#### Senaryo 2: Spec'te Paging VARSA → Repository'de PagedRequest null/default

```csharp
public class GetStudentsWithPagingSpecification : BaseFilteredSpecification<Person>
{
    public GetStudentsWithPagingSpecification(int pageNumber, int pageSize)
    {
        AddInclude(p => p.Student);
        AddCriteria(p => p.Student != null && !p.IsDeleted);
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);  // ← Paging burada
    }
}

// Repository'de PagedRequest default olacak (ignored)
var spec = new GetStudentsWithPagingSpecification(2, 15);
var pagedList = await _repository.GetAllAsync(
    spec,
    new PagedRequest(),  // Default 1, 10
    cancellationToken);
```

**Recommendation:** Spec'te paging YAPMA, Repository'de PagedRequest ile yap.

---

## 🚀 PERFORMANCE OPTIMIZATIONS

### 1. IsSplitQuery (N+1 Query Prevention)

```csharp
// ❌ PROBLEM: 1 main query + N includes = N+1 queries
public class GetStudentsSpecification : BaseFilteredSpecification<Person>
{
    public GetStudentsSpecification()
    {
        AddInclude(p => p.Student);
        AddIncludeString("Student.Enrollments.Course");  // ← Cartesian explosion
    }
}

// ✅ SOLUTION: Split Query
public class GetStudentsOptimizedSpecification : BaseFilteredSpecification<Person>
{
    public GetStudentsOptimizedSpecification()
    {
        AddInclude(p => p.Student);
        AddIncludeString("Student.Enrollments.Course");
        UseSplitQuery();  // ← EF Core multiple queries
    }
}
```

**How it works:**
```sql
-- Query 1: Main query
SELECT * FROM Persons WHERE IsDeleted = 0

-- Query 2: Includes
SELECT * FROM Students WHERE PersonId IN (...)

-- Query 3: Nested includes
SELECT * FROM Enrollments WHERE StudentId IN (...)

-- Query 4: Nested-nested includes
SELECT * FROM Courses WHERE CourseId IN (...)
```

### 2. Projection (Select Minimal Fields)

```csharp
// Repository: Tek soruda SELECT projection'u
public async Task<PagedList<TMap>> GetAllAsync<TMap>(
    ISpecification<TEntity> spec,
    PagedRequest pagedRequest,
    CancellationToken cancellationToken = default)
{
    var query = ApplySpecification(spec);
    // ← HERE: query.Select(e => new TMap { ... })
    
    var totalCount = await query.CountAsync(cancellationToken);
    var items = await query
        .Skip(...)
        .Take(...)
        .ToListAsync(cancellationToken);
    
    var mappedItems = items.Select(x => _mapper.Map<TMap>(x));
    return new PagedList<TMap>(...);
}
```

---

## ⚠️ YAYGRIN HATALAR

| Hata | ❌ | ✅ |
|------|---|---|
| **Method Explosion** | GetStudents(), GetStaffs(), GetActive() | Specification kullan |
| **No Includes** | Lazy load (N+1 queries) | AddInclude() kullan |
| **No Sorting** | Default sırada döner | AddOrderBy() ekle |
| **No Paging** | Tüm records bellekte | ApplyPaging() yap |
| **No Filtering** | Whitelist validation yok | IFilterWhitelist kullan |
| **Deleted Records** | Soft delete check yok | ApplySoftDeleteFilter() |
| **Cartesian Explosion** | Multiple includes → çok data | UseSplitQuery() |
| **Wrong Overload** | Spec ve PagedRequest çakışması | Birini seç |

---

## 📋 CHECKLIST: Yeni Specification Oluştur

- [ ] `BaseFilteredSpecification<T>` inherit et
- [ ] AddInclude() ile eager loading ekle
- [ ] AddCriteria() ile filtering ekle
- [ ] ApplySoftDeleteFilter() ekle (ISoftDelete varsa)
- [ ] ApplyPaging() ekle VEYA Repository'de PagedRequest kullan (bir tanesini seç!)
- [ ] AddOrderBy() ile sorting ekle
- [ ] UseSplitQuery() ekle (nested includes varsa)
- [ ] Specification'ı Query Handler'da kullan
- [ ] Test et (Spec properties'leri kontrol et)

---

**Next:** `CQRS_Handler_Examples.md` → Query/Command handler'ları detaylı örnekler.