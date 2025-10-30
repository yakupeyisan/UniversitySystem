# 🎓 UniversitySystem - START HERE

> **Modular Monolith Architecture with Clean Architecture & CQRS Pattern**

Bu dokuman **UniversitySystem** projesinin mimarisini, patterns'ını ve best practices'ını açıklar. Projeye katılmadan önce **bu dosyayı tamamen okumanız şiddetle tavsiye edilir.**

---

## 📚 OKUMA SIRASI (Tahmini: ~90 dakika)

| # | Dokuman | Süre | İçerik |
|---|---------|------|--------|
| 1 | **START_HERE.md** (Bu dosya) | 25 min | Mimari overview, patterns, best practices |
| 2 | **ProjectStructure.md** | 10 min | Klasör hiyerarşisi detaylı |
| 3 | **Module_TEMPLATE.md** | 15 min | Yeni module oluşturma rehberi |
| 4 | **Specification_Guide.md** | 15 min | Filtering, Sorting, Paging |
| 5 | **CQRS_Handler_Examples.md** | 15 min | Query/Command handler patterns |
| 6 | **Kod İncele** | 15 min | Real-world örnekler repository'de |

---

## 🏗️ MİMARİ OVERVIEW

### **Temel Yapı: Modular Monolith + Clean Architecture**

```
┌─────────────────────────────────────────────────────────────┐
│                   API Layer (Presentation)                   │
│              Controllers + Middlewares + Program.cs           │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│  Core Layer (Shared Infrastructure)                         │
│  ├── Core.Domain (Entities, Specifications, Interfaces)     │
│  ├── Core.Application (Abstractions, Behaviors, Results)    │
│  └── Shared.Infrastructure (Centralized Repository)         │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│  Modules (11 Business Modules - Loosely Coupled)            │
│  ├── PersonMgmt/Domain + PersonMgmt/Application             │
│  ├── Academic/Domain + Academic/Application                 │
│  ├── Identity/Domain + Identity/Application                 │
│  ├── VirtualPOS/Domain + VirtualPOS/Application             │
│  └── ... (8 more modules)                                   │
└─────────────────────────────────────────────────────────────┘
```

**Dependency Flow:** API → Core → Modules (ONE-WAY, NEVER CIRCULAR)

---

## 🎯 KÖK KONSEPTLER

### **1. SPECIFICATION PATTERN - Repository Method Explosion Çözüm**

**❌ ESKI YAKLAŞIM (DRY Violation)**
```csharp
public interface IPersonRepository : IRepository<Person>
{
    Task<List<Person>> GetActivePersonsAsync();
    Task<List<Person>> GetStudentsAsync();
    Task<List<Person>> GetPersonsByAgeAsync(int age);
    Task<Person> GetPersonWithRelationsAsync(Guid id);
    // ... 20+ method
}
```

**✅ YENİ YAKLAŞIM (Specification Pattern)**
```csharp
// Specification tanımla (modül-spesifik)
public class GetActiveStudentsSpecification : BaseFilteredSpecification<Person>
{
    public GetActiveStudentsSpecification(int pageNumber = 1, int pageSize = 20)
    {
        // Filtering
        AddCriteria(p => p.Student != null && !p.IsDeleted && !p.Student.IsDeleted);
        
        // Eager Loading
        AddInclude(p => p.Student);
        AddInclude(p => p.HealthRecord);
        
        // Sorting
        AddOrderBy(p => p.CreatedAt);
        
        // Paging
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
    }
}

// Repository'de kullan (Generic interface)
var spec = new GetActiveStudentsSpecification(pageNumber, pageSize);
var pagedList = await _repository.GetAllAsync(spec, new PagedRequest(1, 10), ct);
```

**Faydalar:**
- ✅ Repository'de sadece **generic methods** (method explosion yok)
- ✅ Her query **modül-spesifik Specification**
- ✅ Filtering, Sorting, Paging, Includes - **bir yerde**
- ✅ Reusable ve testlenebilir

---

### **2. PAGINATION - PagedList<T> + PagedRequest**

**PagedList<T>** (Cevap modeli)
```csharp
public class PagedList<T>
{
    public IReadOnlyCollection<T> Data { get; }      // Immutable items
    public int TotalCount { get; }                   // Toplam record sayısı
    public int PageNumber { get; }
    public int PageSize { get; }
    public int TotalPages { get; }                   // Auto-calculated
    public bool HasNextPage { get; }                 // Client navigation
    public bool HasPreviousPage { get; }
}
```

**PagedRequest** (Sorgu parametresi)
```csharp
public class PagedRequest
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; }              // Opsiyonel sorting
    public string? SortDirection { get; set; } = "asc";
    
    public bool IsValid() { }                        // Validation
    public int GetSkipCount() { }                    // Helper
}
```

**Kullanım:**
```csharp
// Query Handler'da
var spec = new GetStudentsSpecification(query.PageNumber, query.PageSize);
var pagedList = await _repository.GetAllAsync(
    spec,                              // Specification (filtering + sorting)
    query.PagedRequest,                // Pagination
    cancellationToken);

// Response
return new PagedList<StudentDto>(
    mappedData,
    pagedList.TotalCount,
    pagedList.PageNumber,
    pagedList.PageSize);
```

---

### **3. FILTERING - Dynamic + Safe**

**BaseFilteredSpecification** ile advanced filtering:

```csharp
// PersonFilterWhitelist (Hangi field'lar filtrelenebilir?)
public class PersonFilterWhitelist : IFilterWhitelist
{
    private static readonly HashSet<string> AllowedProperties = new()
    {
        nameof(Person.FirstName),
        nameof(Person.LastName),
        nameof(Person.Email),
        nameof(Person.PersonType),
        // ... güvenli field'lar
    };
}

// Specification'da kullan
public class GetPersonsWithFiltersSpecification 
    : BaseFilteredSpecification<Person>
{
    public GetPersonsWithFiltersSpecification(
        string? filterString,
        int pageNumber = 1,
        int pageSize = 20)
    {
        AddInclude(p => p.Student);
        AddInclude(p => p.HealthRecord);
        
        // filterString'i parse et (whitelist ile validation)
        if (!string.IsNullOrWhiteSpace(filterString))
        {
            var filterParser = new FilterParser<Person>(
                whitelist: new PersonFilterWhitelist());
            var filterExpression = filterParser.Parse(filterString);
            AddCriteria(filterExpression);
        }
        
        ApplySoftDeleteFilter();          // ISoftDelete entities
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
    }
}
```

**API Call örneği:**
```
GET /api/persons?pageNumber=1&pageSize=10&filterString=firstName=John;email=@gmail.com
```

---

### **4. SORTING - Specification'da Tanımlı**

```csharp
public class GetStudentsSpecification : BaseFilteredSpecification<Person>
{
    public GetStudentsSpecification()
    {
        // Default sorting
        AddOrderBy(p => p.Student!.StudentNumber);
    }
}

public class GetPersonsByCreatedSpecification : BaseFilteredSpecification<Person>
{
    public GetPersonsByCreatedSpecification(bool descending = false)
    {
        if (descending)
            AddOrderByDescending(p => p.CreatedAt);
        else
            AddOrderBy(p => p.CreatedAt);
    }
}
```

**Specification'da MULTIPLE sort field'lar:**
```csharp
AddOrderBy(p => p.LastName);              // First sort
AddOrderByDescending(p => p.FirstName);   // Then descending
AddOrderBy(p => p.CreatedAt);             // Then ascending
```

---

### **5. EAGER LOADING - Specification'da**

**Expression-based** (Recommended):
```csharp
AddInclude(p => p.Student);
AddInclude(p => p.HealthRecord);
AddInclude(p => p.Restrictions);
```

**String-based** (Nested navigation):
```csharp
AddIncludeString("Student.Enrollments.Course");
AddIncludeString("HealthRecord.MedicalConditions");
```

**Performance:** IsSplitQuery ile N+1 prevention
```csharp
public class GetStudentsWithEnrollmentsSpec : BaseFilteredSpecification<Person>
{
    public GetStudentsWithEnrollmentsSpec()
    {
        AddInclude(p => p.Student);
        AddIncludeString("Student.Enrollments.Course");
        
        UseSplitQuery();  // ← EF Core split queries kullan
    }
}
```

---

### **6. REPOSITORY INTERFACE - Overload Flexibility**

**IRepository<TEntity>** interface'i:

```csharp
// 🔹 Single record operations
Task<TEntity?> GetByIdAsync(Guid id, CancellationToken = default);
Task<TMap?> GetByIdAsync<TMap>(Guid id, CancellationToken = default);

// 🔹 Get with Specification
Task<TEntity?> GetAsync(ISpecification<TEntity> spec, CancellationToken = default);
Task<TMap?> GetAsync<TMap>(ISpecification<TEntity> spec, CancellationToken = default);

// 🔹 All records (no paging)
Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken = default);
Task<IEnumerable<TEntity>> GetAllAsync(ISpecification<TEntity> spec, CancellationToken = default);

// 🔹 Paged - WITHOUT Specification
Task<PagedList<TEntity>> GetAllAsync(PagedRequest pagedRequest, CancellationToken = default);
Task<PagedList<TMap>> GetAllAsync<TMap>(PagedRequest pagedRequest, CancellationToken = default);

// 🔹 Paged + Specification (MOST USED!)
Task<PagedList<TEntity>> GetAllAsync(
    ISpecification<TEntity> spec,
    PagedRequest pagedRequest,
    CancellationToken = default);
Task<PagedList<TMap>> GetAllAsync<TMap>(
    ISpecification<TEntity> spec,
    PagedRequest pagedRequest,
    CancellationToken = default);

// 🔹 Checks
Task<bool> ExistsAsync(Guid id, CancellationToken = default);
Task<bool> ExistsAsync(ISpecification<TEntity> spec, CancellationToken = default);
Task<bool> IsUniqueAsync(ISpecification<TEntity> spec, CancellationToken = default);

// 🔹 Count
Task<int> CountAsync(CancellationToken = default);
Task<int> CountAsync(ISpecification<TEntity> spec, CancellationToken = default);

// 🔹 Write operations
Task AddAsync(TEntity aggregate, CancellationToken = default);
Task AddRangeAsync(IEnumerable<TEntity> aggregates, CancellationToken = default);
Task UpdateAsync(TEntity aggregate, CancellationToken = default);
Task DeleteAsync(TEntity aggregate, CancellationToken = default);
Task DeleteRangeAsync(IEnumerable<TEntity> aggregates, CancellationToken = default);
Task SaveChangesAsync(CancellationToken = default);
```

---

### **7. CQRS HANDLERS - QUERY ÖRNEĞI**

```csharp
// Query tanım
public class GetAllPersonsQuery : IRequest<Result<PagedList<PersonResponse>>>
{
    public string? FilterString { get; set; }
    public PagedRequest PagedRequest { get; set; }

    public GetAllPersonsQuery(string? filterString, PagedRequest pagedRequest)
    {
        FilterString = filterString;
        PagedRequest = pagedRequest ?? new PagedRequest();
    }
}

// Handler implementasyon
public class Handler : IRequestHandler<GetAllPersonsQuery, Result<PagedList<PersonResponse>>>
{
    private readonly IRepository<Person> _personRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<Handler> _logger;

    public Handler(
        IRepository<Person> personRepository,
        IMapper mapper,
        ILogger<Handler> logger)
    {
        _personRepository = personRepository ?? throw new ArgumentNullException(nameof(personRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<PagedList<PersonResponse>>> Handle(
        GetAllPersonsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Fetching persons - Filter: {FilterString}, Page: {PageNumber}/{PageSize}",
                request.FilterString ?? "none",
                request.PagedRequest.PageNumber,
                request.PagedRequest.PageSize);

            // 1️⃣ Specification oluştur
            var spec = new GetPersonsWithFiltersSpecification(
                request.FilterString,
                request.PagedRequest.PageNumber,
                request.PagedRequest.PageSize);

            // 2️⃣ Repository'den veri çek
            var pagedList = await _personRepository.GetAllAsync(
                spec,
                request.PagedRequest,
                cancellationToken);

            // 3️⃣ Entity → DTO mapping
            var responses = _mapper.Map<List<PersonResponse>>(pagedList.Data);

            // 4️⃣ PagedList<T> wrap et
            var result = new PagedList<PersonResponse>(
                responses,
                pagedList.TotalCount,
                pagedList.PageNumber,
                pagedList.PageSize);

            _logger.LogInformation(
                "Retrieved {Count} persons (Total: {Total})",
                responses.Count,
                result.TotalCount);

            return Result<PagedList<PersonResponse>>.Success(
                result,
                $"Retrieved {responses.Count} items from {result.TotalCount} total");
        }
        catch (FilterParsingException ex)
        {
            _logger.LogWarning(ex, "Filter parsing error: {Filter}", request.FilterString);
            return Result<PagedList<PersonResponse>>.Failure($"Invalid filter: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error fetching persons");
            return Result<PagedList<PersonResponse>>.Failure("An unexpected error occurred");
        }
    }
}

// API Controller
[ApiController]
[Route("api/[controller]")]
public class PersonsController : ControllerBase
{
    private readonly IMediator _mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? filterString = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllPersonsQuery(
            filterString,
            new PagedRequest { PageNumber = pageNumber, PageSize = pageSize });

        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }
}
```

---

## 📂 PROJE YAPISI

### **Core Layer**
```
src/Core/
├── Core.Domain/
│   ├── Entities/              # Entity base class
│   ├── Specifications/        # BaseSpecification, ISpecification
│   ├── Pagination/            # PagedList<T>, PagedRequest
│   ├── Repositories/          # IRepository<T> interface
│   ├── Filtering/             # IFilterParser, IFilterWhitelist
│   ├── Results/               # Result<T> wrapper
│   └── Common/                # Shared constants, enums
│
├── Core.Application/
│   ├── Abstractions/          # IApplicationService, IDateTime
│   ├── Behaviors/             # MediatR pipeline behaviors
│   ├── Exceptions/            # App-level exceptions
│   └── Extensions/            # Extension methods
│
└── Shared.Infrastructure/
    └── Persistence/
        ├── Repositories/      # Repository<T> implementation
        ├── Contexts/          # AppDbContext (CENTRALIZED)
        ├── Configurations/    # EF entity configurations
        ├── Migrations/        # All migrations
        └── Extensions/        # DI registration
```

### **Module Structure** (Her module aynı pattern)
```
src/Modules/[ModuleName]/
├── Domain/
│   ├── Aggregates/           # Aggregate roots, entities
│   ├── Enums/
│   ├── Events/
│   ├── Exceptions/
│   ├── Specifications/       # Module-specific specs
│   ├── Filtering/            # Module-specific filters + whitelist
│   └── Interfaces/
│
├── Application/
│   ├── Queries/              # IRequest handlers
│   ├── Commands/             # IRequest handlers
│   ├── DTOs/
│   ├── Validators/           # FluentValidation
│   ├── Mappers/              # AutoMapper profiles
│   └── Extensions/           # ServiceCollectionExtensions
│
└── [API] (optional, sadece VirtualPOS'ta)
```

### **Presentation Layer**
```
src/Presentation/
└── API/
    ├── Controllers/          # [ApiController]
    ├── Middlewares/
    │   ├── ExceptionHandlerMiddleware
    │   └── LoggingMiddleware
    ├── Extensions/
    ├── Program.cs            # Startup, DI registration
    └── appsettings.json
```

---

## ✅ BEST PRACTICES

### **1. Repository Kullanımı**
```csharp
// ✅ DOĞRU - Specification pattern
var spec = new GetActiveStudentsSpecification();
var students = await _repository.GetAllAsync(spec, new PagedRequest(1, 10), ct);

// ❌ YANLIŞ - Custom method
var students = await _repository.GetActiveStudentsAsync(1, 10);

// ❌ YANLIŞ - DbContext direkti
var students = await _context.Persons
    .Where(p => p.Student != null)
    .ToListAsync();
```

### **2. Specification Oluşturma**
```csharp
// ✅ DOĞRU - Başında ApplyPaging
public class MySpecification : BaseFilteredSpecification<Entity>
{
    public MySpecification(int pageNumber = 1, int pageSize = 20)
    {
        AddCriteria(...);
        AddInclude(...);
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
        AddOrderBy(...);
    }
}

// ❌ YANLIŞ - Skip paging
public class MySpecification : BaseFilteredSpecification<Entity>
{
    public MySpecification()
    {
        // Paging yok, repository'de manual pagination yapılacak
        AddCriteria(...);
    }
}
```

### **3. CQRS Handler'ı**
```csharp
// ✅ DOĞRU - Try-catch + logging
public async Task<Result<PagedList<Dto>>> Handle(..., CancellationToken ct)
{
    try
    {
        _logger.LogInformation("Starting operation");
        var result = await _repository.GetAllAsync(spec, request.PagedRequest, ct);
        return Result<PagedList<Dto>>.Success(mappedResult);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error occurred");
        return Result<PagedList<Dto>>.Failure(ex.Message);
    }
}

// ❌ YANLIŞ - Exception throw etmek
public async Task<PagedList<Dto>> Handle(..., CancellationToken ct)
{
    var result = await _repository.GetAllAsync(spec, request.PagedRequest, ct);
    return mappedResult;  // Exception controller'a gidecek
}
```

### **4. Module Bağımsızlığı**
```csharp
// ✅ DOĞRU - Modules arası loose coupling
// PersonMgmt → Academic yapmasın
var person = await _personRepository.GetByIdAsync(personId, ct);
// Gerekli bilgi için Academic API'yi çağır

// ❌ YANLIŞ - Modülü reference ettirmek
using Academic.Domain;  // ← Never!
var course = await _courseRepository.GetByIdAsync(courseId, ct);
```

### **5. DI Registration**
```csharp
// ✅ DOĞRU - Merkezi AppDbContext
services.AddScoped<IRepository<TEntity>, Repository<TEntity>>();
services.AddDbContext<AppDbContext>(...);  // Merkezi

// ❌ YANLIŞ - Her modülün kendi DbContext'i
services.AddDbContext<PersonMgmtDbContext>(...);
services.AddDbContext<AcademicDbContext>(...);
```

---

## 🔧 ORTAK İŞLEMLER

### **Yeni Specification Oluşturma**
```csharp
namespace [Module].Domain.Specifications;

public class Get[Entity]WithFiltersSpecification : BaseFilteredSpecification<[Entity]>
{
    public Get[Entity]WithFiltersSpecification(
        string? filterString = null,
        int pageNumber = 1,
        int pageSize = 20)
    {
        // 1. Includes
        AddInclude(e => e.RelatedEntity);
        
        // 2. Base criteria
        AddCriteria(e => !e.IsDeleted);
        
        // 3. Dynamic filtering
        if (!string.IsNullOrWhiteSpace(filterString))
        {
            var parser = new FilterParser<[Entity]>(
                whitelist: new [Entity]FilterWhitelist());
            AddCriteria(parser.Parse(filterString));
        }
        
        // 4. Soft delete (ISoftDelete implements'te)
        ApplySoftDeleteFilter();
        
        // 5. Paging
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
        
        // 6. Sorting
        AddOrderBy(e => e.PropertyName);
    }
}
```

### **Yeni Query Handler Oluşturma**
```csharp
public class Get[Entity]Query : IRequest<Result<PagedList<[Entity]Dto>>>
{
    public string? FilterString { get; set; }
    public PagedRequest PagedRequest { get; set; }
}

public class Handler : IRequestHandler<Get[Entity]Query, Result<PagedList<[Entity]Dto>>>
{
    private readonly IRepository<[Entity]> _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<Handler> _logger;

    public async Task<Result<PagedList<[Entity]Dto>>> Handle(Get[Entity]Query request, CancellationToken ct)
    {
        try
        {
            _logger.LogInformation("Fetching [Entity] - Filter: {Filter}", request.FilterString ?? "none");

            var spec = new Get[Entity]WithFiltersSpecification(
                request.FilterString,
                request.PagedRequest.PageNumber,
                request.PagedRequest.PageSize);

            var pagedList = await _repository.GetAllAsync(spec, request.PagedRequest, ct);
            var responses = _mapper.Map<List<[Entity]Dto>>(pagedList.Data);
            var result = new PagedList<[Entity]Dto>(responses, pagedList.TotalCount, pagedList.PageNumber, pagedList.PageSize);

            return Result<PagedList<[Entity]Dto>>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching [Entity]");
            return Result<PagedList<[Entity]Dto>>.Failure(ex.Message);
        }
    }
}
```

---

## ⚠️ YAYGIN HATALAR

| Hata | ❌ YANLIŞ | ✅ DOĞRU |
|------|----------|---------|
| **Method Explosion** | `GetStudentsAsync()`, `GetActiveAsync()`, ... | Specification pattern kullan |
| **DbContext Direkti** | `_context.Set<T>().Where(...)` | Repository + Specification |
| **No Result Wrapper** | `throw new Exception()` | `Result<T>.Failure(message)` |
| **Circular References** | Module A → Module B → Module A | One-way dependency |
| **No Logging** | Sessiz başarı/başarısızlık | Her critical point'te log |
| **No Validation** | `pagedRequest.PageSize = -5` | `IsValid()` check eder |
| **Nested Includes** | `AddIncludeString(...)` sonra `Include()` | Bir method seç |
| **No Soft Delete Check** | Deleted records döndürme | `ApplySoftDeleteFilter()` |

---

## 📖 KOD ÖRNEKLERİ

**Repository'de:**
```csharp
protected IQueryable<TEntity> ApplySpecification(ISpecification<TEntity> spec)
{
    var query = _context.Set<TEntity>().AsQueryable();
    
    if (spec.IsSplitQuery)
        query = query.AsSplitQuery();
    
    if (spec.Criteria != null)
        query = query.Where(spec.Criteria);
    
    if (spec.Includes?.Any() == true)
        query = spec.Includes.Aggregate(query, (current, include) => current.Include(include));
    
    if (spec.IncludeStrings?.Any() == true)
        query = spec.IncludeStrings.Aggregate(query, (current, include) => current.Include(include));
    
    if (spec.OrderBys?.Any() == true)
        foreach (var (orderExpression, isDescending) in spec.OrderBys)
            query = isDescending 
                ? query.OrderByDescending(orderExpression) 
                : query.OrderBy(orderExpression);
    
    return query;
}

public async Task<PagedList<TEntity>> GetAllAsync(
    ISpecification<TEntity> specification,
    PagedRequest pagedRequest,
    CancellationToken cancellationToken = default)
{
    var query = ApplySpecification(specification);
    var totalCount = await query.CountAsync(cancellationToken);
    var items = await query
        .Skip((pagedRequest.PageNumber - 1) * pagedRequest.PageSize)
        .Take(pagedRequest.PageSize)
        .ToListAsync(cancellationToken);
    
    return new PagedList<TEntity>(items, totalCount, pagedRequest.PageNumber, pagedRequest.PageSize);
}
```

---

## 🚀 HEMEN BAŞLAMAK İÇİN

### **1️⃣ Yeni Module Oluştur**
```bash
# Folder struktur:
src/Modules/[ModuleName]/
├── Domain/
│   ├── Aggregates/
│   ├── Specifications/
│   ├── Filtering/
│   └── Exceptions/
└── Application/
    ├── Queries/
    ├── Commands/
    ├── DTOs/
    ├── Validators/
    └── Mappers/
```

### **2️⃣ Domain Entity Oluştur**
```csharp
public class MyEntity : Entity
{
    public string Name { get; set; }
}
public class MyEntity : AuditableEntity
{
    public string Name { get; set; }
}
```

### **3️⃣ Specification Oluştur**
```csharp
public class GetMyEntitiesSpecification : BaseFilteredSpecification<MyEntity>
{
    public GetMyEntitiesSpecification(int pageNumber = 1, int pageSize = 20)
    {
        AddCriteria(e => !e.IsDeleted);
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
        AddOrderBy(e => e.CreatedAt);
    }
}
```

### **4️⃣ Query Handler Oluştur**
```csharp
public class GetMyEntitiesQuery : IRequest<Result<PagedList<MyEntityDto>>> { }

public class Handler : IRequestHandler<GetMyEntitiesQuery, Result<PagedList<MyEntityDto>>>
{
    public async Task<Result<PagedList<MyEntityDto>>> Handle(GetMyEntitiesQuery request, CancellationToken ct)
    {
        var spec = new GetMyEntitiesSpecification();
        var result = await _repository.GetAllAsync(spec, new PagedRequest(), ct);
        // Map & return...
    }
}
```

---

## 💡 SONUÇ

Bu mimari **scalable, maintainable ve test-friendly**:

✅ **Modular:** Her module bağımsız, loosely coupled  
✅ **Clean:** Clear separation of concerns  
✅ **CQRS:** Commands ve Queries ayrı  
✅ **Spec Pattern:** Repository explosion yok  
✅ **Pagination:** PagedList + PagedRequest standard  
✅ **Filtering:** Dynamic, safe, whitelist-validated  
✅ **Sorting:** Specification'da tanımlı  
✅ **Eager Loading:** Spec'de Include'lar  
✅ **Performance:** IsSplitQuery, soft delete, lazy load  
✅ **Testable:** Mock'lanabilir interfaces  

**Mimariyi bozmamak için:**
1. Her zaman Specification pattern kullan
2. DbContext direkti erişim yapma
3. Module'ler arası bidirectional dependency kurma
4. Generic Repository kullan
5. Result<T> wrapper ile error handling yap
6. Logging ekle
7. Validation kontrol et

---

**Next:** `ProjectStructure.md` dokumentasyonunu oku → `Module_TEMPLATE.md` ile yeni module oluştur → Kod örneklerini incele.
