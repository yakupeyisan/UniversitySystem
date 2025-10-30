# 📂 ProjectStructure.md - Klasör Hiyerarşisi Detaylı

## 🎯 Genel Yapı

```
UniversitySystem/
├── src/                              # Tüm kaynak kod
│   ├── Core/                         # Shared core infrastructure
│   ├── Modules/                      # Business modules (11 adet)
│   ├── Presentation/                 # API presentation
│   └── Shared/                       # Shared utilities
│
├── tests/                            # Test projeler
│   ├── Unit/
│   └── Integration/
│
├── docs/                             # Documentation & diagrams
├── UniversitySystem.sln              # Solution file
└── README.md
```

---

## 📁 src/Core/ - Temel Altyapı

### Core.Domain.csproj
```
src/Core/Core.Domain/
├── Entities/
│   ├── Entity.cs                     # Base entity (Guid Id, CreatedAt, UpdatedAt)
│   ├── IAggregateRoot.cs             # Marker interface
│   └── ISoftDelete.cs                # Soft delete marker
│
├── Specifications/
│   ├── ISpecification<T>.cs          # Specification interface
│   ├── BaseSpecification<T>.cs       # Base implementation
│   ├── BaseFilteredSpecification<T>.cs   # Filtering + paging
│   └── Specification<T>.cs           # Alternative base
│
├── Pagination/
│   ├── PagedList<T>.cs               # Response model (immutable)
│   └── PagedRequest.cs               # Query parameter model
│
├── Repositories/
│   ├── IRepository<T>.cs             # Generic repository interface
│   └── IUnitOfWork.cs                # (opsiyonel)
│
├── Filtering/
│   ├── IFilterParser<T>.cs           # Dynamic filtering
│   ├── IFilterWhitelist.cs           # Security - allowed properties
│   ├── IFilterExpressionBuilder<T>.cs
│   └── FilterExpression.cs           # Filter object model
│
├── Results/
│   ├── Result<T>.cs                  # Success/Failure wrapper
│   ├── Error.cs                      # Error details
│   └── ValidationError.cs            # Validation errors
│
└── Common/
    ├── Constants/
    └── Enums/
```

**Sorumlu:** Business domain tanımları, interfaces, base classes

---

### Core.Application.csproj
```
src/Core/Core.Application/
├── Abstractions/
│   ├── IApplicationService.cs        # Marker interface
│   ├── IDateTime.cs                  # DateTime abstraction
│   └── ICurrentUser.cs               # Current user info
│
├── Behaviors/
│   ├── ValidationPipelineBehavior.cs    # MediatR pipeline
│   └── LoggingPipelineBehavior.cs
│
├── Exceptions/
│   ├── ApplicationException.cs        # Base app exception
│   ├── ValidationException.cs         # FluentValidation errors
│   └── NotFoundException.cs
│
└── Extensions/
    ├── StringExtensions.cs
    └── CollectionExtensions.cs
```

**Sorumlu:** Cross-cutting concerns, application-level abstractions

---

## 📁 src/Shared/ - Centralized Infrastructure

### Shared.Infrastructure.csproj
```
src/Shared/Shared.Infrastructure/
└── Persistence/
    ├── Repositories/
    │   ├── Repository<T>.cs          # Generic repository implementation
    │   │   ├── ApplySpecification()
    │   │   ├── GetByIdAsync()
    │   │   ├── GetAsync()
    │   │   ├── GetAllAsync() (5 overloads)
    │   │   ├── ExistsAsync()
    │   │   ├── CountAsync()
    │   │   └── Add/Update/Delete()
    │   └── RepositoryFactory.cs
    │
    ├── Contexts/
    │   └── AppDbContext.cs           # CENTRALIZED DbContext
    │       └── DbSet<T> for ALL entities
    │
    ├── Configurations/
    │   ├── PersonConfiguration.cs
    │   ├── StudentConfiguration.cs
    │   ├── StaffConfiguration.cs
    │   ├── CourseConfiguration.cs
    │   └── ... (tüm entity configurations)
    │
    ├── Migrations/
    │   ├── 20240101_InitialCreate.cs
    │   ├── 20240102_AddXFeature.cs
    │   └── ...
    │
    └── Extensions/
        └── ServiceCollectionExtensions.cs
            ├── AddDatabaseContext()
            ├── AddRepositories()
            └── AddInfrastructure()
```

**Sorumlu:** 
- ✅ Generic Repository implementation
- ✅ Centralized AppDbContext (tüm modules)
- ✅ Entity Framework configurations
- ✅ Database migrations
- ✅ DI registration

**ÖNEMLİ:** Tüm entities AppDbContext'de DbSet olarak bulunur!

```csharp
public class AppDbContext : DbContext
{
    // PersonMgmt entities
    public DbSet<Person> Persons { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Staff> Staffs { get; set; }
    
    // Academic entities
    public DbSet<Course> Courses { get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }
    
    // ... tüm modules
}
```

---

## 📁 src/Modules/ - Business Modules (Loosely Coupled)

Her module **aynı pattern**:

```
src/Modules/[ModuleName]/
├── Domain/
│   ├── Aggregates/
│   │   ├── [AggregateRoot].cs        # Main aggregate
│   │   └── Entities/
│   │       ├── [Entity1].cs
│   │       └── [Entity2].cs
│   │
│   ├── ValueObjects/
│   │   ├── [ValueObject1].cs
│   │   └── [ValueObject2].cs
│   │
│   ├── Events/
│   │   ├── [AggregateCreatedEvent].cs
│   │   └── [AggregateUpdatedEvent].cs
│   │
│   ├── Exceptions/
│   │   ├── [AggregateNotFoundException].cs
│   │   └── [InvalidOperationException].cs
│   │
│   ├── Specifications/              # ← Module-specific!
│   │   ├── GetActiveXSpecification.cs
│   │   ├── GetXWithFiltersSpecification.cs
│   │   └── GetXByStatusSpecification.cs
│   │
│   ├── Filtering/                   # ← Module-specific!
│   │   ├── XFilterParser.cs         # (opsiyonel)
│   │   ├── XFilterWhitelist.cs      # Allowed filter properties
│   │   └── XFilterExpressions.cs
│   │
│   └── Interfaces/
│       └── IXRepository.cs          # (opsiyonel, genellikle generic kullan)
│
├── Application/
│   ├── Queries/
│   │   ├── GetXQuery.cs             # IRequest<Result<T>>
│   │   ├── GetAllXQuery.cs          # IRequest<Result<PagedList<T>>>
│   │   └── [QueryName]Query.cs
│   │
│   ├── Commands/
│   │   ├── CreateXCommand.cs        # IRequest<Result<T>>
│   │   ├── UpdateXCommand.cs
│   │   ├── DeleteXCommand.cs
│   │   └── [CommandName]Command.cs
│   │
│   ├── DTOs/
│   │   ├── Request/
│   │   │   └── [EntityName]Request.cs
│   │   └── Response/
│   │       └── [EntityName]Response.cs
│   │
│   ├── Validators/
│   │   ├── CreateXCommandValidator.cs
│   │   └── UpdateXCommandValidator.cs
│   │
│   ├── Mappers/
│   │   └── [EntityName]MappingProfile.cs
│   │
│   └── Extensions/
│       └── ServiceCollectionExtensions.cs
│           ├── Add[Module]Queries()
│           ├── Add[Module]Commands()
│           └── Add[Module]Services()
│
└── [Module].Domain.csproj
    └── [Module].Application.csproj
```

### Module Örneği: PersonMgmt
```
src/Modules/PersonMgmt/
├── Domain/
│   ├── Aggregates/Person.cs
│   ├── Entities/
│   │   ├── Student.cs
│   │   ├── Staff.cs
│   │   ├── HealthRecord.cs
│   │   └── PersonRestriction.cs
│   ├── ValueObjects/PersonName.cs, Address.cs
│   ├── Events/PersonCreated, PersonUpdated, etc.
│   ├── Exceptions/PersonNotFoundException, etc.
│   ├── Specifications/
│   │   ├── GetActivePersonsSpecification.cs
│   │   ├── GetPersonsWithFiltersSpecification.cs
│   │   └── GetStudentsSpecification.cs
│   ├── Filtering/
│   │   ├── PersonFilterWhitelist.cs
│   │   └── PersonFilterExpressions.cs
│   └── Interfaces/IPersonRepository.cs (opsiyonel)
│
└── Application/
    ├── Queries/
    │   ├── GetPersonQuery.cs
    │   ├── GetAllPersonsQuery.cs
    │   └── GetStudentsQuery.cs
    ├── Commands/
    │   ├── CreatePersonCommand.cs
    │   ├── UpdatePersonCommand.cs
    │   └── DeletePersonCommand.cs
    ├── DTOs/
    ├── Validators/
    ├── Mappers/PersonMappingProfile.cs
    └── Extensions/ServiceCollectionExtensions.cs
```

---

## 📁 src/Presentation/ - API Layer

```
src/Presentation/
└── API/
    ├── Controllers/
    │   ├── PersonsController.cs      # [ApiController] PersonMgmt
    │   ├── CoursesController.cs      # [ApiController] Academic
    │   ├── PaymentsController.cs     # [ApiController] VirtualPOS
    │   └── [EntityName]Controller.cs
    │
    ├── Middlewares/
    │   ├── ExceptionHandlerMiddleware.cs
    │   │   └── Try/catch global error handling
    │   ├── LoggingMiddleware.cs
    │   │   └── Request/response logging
    │   ├── AuthenticationMiddleware.cs (opsiyonel)
    │   └── CorsMiddleware.cs (opsiyonel)
    │
    ├── Extensions/
    │   ├── ServiceCollectionExtensions.cs
    │   │   ├── AddCoreServices()
    │   │   ├── AddApplicationServices()
    │   │   ├── AddInfrastructure()
    │   │   ├── AddModules()
    │   │   └── AddSwagger()
    │   │
    │   └── IApplicationBuilderExtensions.cs
    │       ├── UseSwagger()
    │       ├── UseMiddlewares()
    │       └── UseCors()
    │
    ├── Program.cs               # Startup configuration
    │   ├── var builder = WebApplication.CreateBuilder(args);
    │   ├── builder.Services.Add...()
    │   ├── var app = builder.Build();
    │   ├── app.Use...()
    │   └── app.Run();
    │
    ├── appsettings.json         # Config (local)
    ├── appsettings.Development.json
    ├── appsettings.Production.json
    └── API.csproj
```

---

## 📁 tests/ - Test Projeler

```
tests/
├── Unit/
│   ├── Tests.Unit.csproj
│   ├── PersonMgmt/
│   │   ├── Domain/
│   │   │   └── PersonAggregateTests.cs
│   │   ├── Application/
│   │   │   └── GetAllPersonsQueryHandlerTests.cs
│   │   └── Specifications/
│   │       └── GetPersonsSpecificationTests.cs
│   │
│   ├── Academic/
│   └── ...
│
└── Integration/
    ├── Tests.Integration.csproj
    ├── PersonMgmt/
    │   ├── Repositories/
    │   │   └── PersonRepositoryTests.cs
    │   └── Queries/
    │       └── GetAllPersonsQueryTests.cs
    │
    ├── Academic/
    └── ...
```

---

## 📊 Dependency Graph

```
┌─────────────────────────────────────────┐
│  API (Program.cs, Controllers)          │
└─────────────────┬───────────────────────┘
                  ↓
┌─────────────────────────────────────────┐
│  Core.Application (Behaviors, Results)  │
└─────────────────┬───────────────────────┘
                  ↓
┌─────────────────────────────────────────┐
│  Modules.Application (Queries, Commands)│
└─────────────────┬───────────────────────┘
                  ↓
┌─────────────────────────────────────────┐
│  Modules.Domain (Entities, Specs)       │
│  Core.Domain (Interfaces)               │
└─────────────────┬───────────────────────┘
                  ↓
┌─────────────────────────────────────────┐
│  Shared.Infrastructure (Repository)     │
│  + Core.Infrastructure (AppDbContext)   │
└─────────────────────────────────────────┘
                  ↓
┌─────────────────────────────────────────┐
│  Entity Framework Core + SQL Server     │
└─────────────────────────────────────────┘
```

**KURAL:** Her katman sadece altındaki katmanları referans edebilir!
- ❌ Modules.Domain → API referans etme
- ❌ Core.Application → Modules referans etme (except as generic interfaces)

---

## ✅ Checklist: Yeni Module Oluştururken

- [ ] Module klasörü oluştur: `src/Modules/[ModuleName]/`
- [ ] Domain project oluştur: `[ModuleName].Domain.csproj`
- [ ] Application project oluştur: `[ModuleName].Application.csproj`
- [ ] Aggregate root ekle: `Domain/Aggregates/[Entity].cs`
- [ ] Specifications ekle: `Domain/Specifications/Get[Entity]Specification.cs`
- [ ] FilterWhitelist ekle: `Domain/Filtering/[Entity]FilterWhitelist.cs`
- [ ] Query ekle: `Application/Queries/GetAll[Entity]Query.cs`
- [ ] DTOs ekle: `Application/DTOs/[Entity]Request.cs`, `[Entity]Response.cs`
- [ ] Mapper ekle: `Application/Mappers/[Entity]MappingProfile.cs`
- [ ] ServiceCollectionExtensions ekle: `Application/Extensions/ServiceCollectionExtensions.cs`
- [ ] AppDbContext'e DbSet ekle: `Shared.Infrastructure/Contexts/AppDbContext.cs`
- [ ] Entity Configuration ekle: `Shared.Infrastructure/Persistence/Configurations/[Entity]Configuration.cs`
- [ ] Controller ekle: `src/Presentation/API/Controllers/[Entity]Controller.cs`
- [ ] Program.cs'de DI register et

🎉 **Yeni module hazır!**