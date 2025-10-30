# 🎯 CQRS_Handler_Examples.md - Query & Command Handlers

## 📌 CQRS Pattern Nedir?

**CQRS = Command Query Responsibility Segregation**

- **Commands:** Veri değiştiren işlemler (Create, Update, Delete) → Yazma
- **Queries:** Veri okuma işlemleri (Get, GetAll, Search) → Okuma

Aynı model ile hem yazma hem okuma olmaz. **Ayrı handler'lar, ayrı lojikler.**

### ✨ Proje Pattern: Handler'lar Nested Class

Bu projede Handler'lar **Query/Command class'ının içinde nested class** olarak yazılırlar. Bu çok daha **organized ve clean**!

```csharp
public class GetPersonQuery : IRequest<Result<PersonResponse>>
{
    // Query properties
    public Guid PersonId { get; set; }

    // ✅ Handler nested class
    public class Handler : IRequestHandler<GetPersonQuery, Result<PersonResponse>>
    {
        // Handler implementation
    }
}
```

---

## 🔍 QUERY HANDLERS - Okuma İşlemleri

### Örnek 1: GetPersonQuery (Single Record)

```csharp
// PersonMgmt.Application/Queries/GetPersonQuery.cs
using AutoMapper;
using Core.Domain.Repositories;
using Core.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using PersonMgmt.Application.DTOs;
using PersonMgmt.Domain.Aggregates;
using PersonMgmt.Domain.Specifications;

namespace PersonMgmt.Application.Queries;

public class GetPersonQuery : IRequest<Result<PersonResponse>>
{
    public GetPersonQuery(Guid personId)
    {
        PersonId = personId;
    }

    public Guid PersonId { get; set; }

    // ✅ Handler nested class
    public class Handler : IRequestHandler<GetPersonQuery, Result<PersonResponse>>
    {
        private readonly ILogger<Handler> _logger;
        private readonly IMapper _mapper;
        private readonly IRepository<Person> _personRepository;

        public Handler(
            IRepository<Person> personRepository,
            IMapper mapper,
            ILogger<Handler> logger)
        {
            _personRepository = personRepository ?? throw new ArgumentNullException(nameof(personRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<PersonResponse>> Handle(
            GetPersonQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching person with ID: {PersonId}", request.PersonId);

                // 1. Specification
                var spec = new PersonByIdSpecification(request.PersonId);

                // 2. Repository call
                var person = await _personRepository.GetAsync(spec, cancellationToken);

                if (person == null)
                {
                    _logger.LogWarning("Person not found with ID: {PersonId}", request.PersonId);
                    return Result<PersonResponse>.Failure($"Person with ID {request.PersonId} not found");
                }

                // 3. Mapping
                var response = _mapper.Map<PersonResponse>(person);

                // 4. Success
                _logger.LogInformation("Retrieved person successfully with ID: {PersonId}", person.Id);
                return Result<PersonResponse>.Success(response, "Person retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching person with ID: {PersonId}", request.PersonId);
                return Result<PersonResponse>.Failure(ex.Message);
            }
        }
    }
}

// API Controller
[ApiController]
[Route("api/[controller]")]
public class PersonsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PersonsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetPerson(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var query = new GetPersonQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return NotFound(result);

        return Ok(result);
    }
}
```

---

### Örnek 2: GetAllPersonsQuery (Paginated List with Filtering)

```csharp
// PersonMgmt.Application/Queries/GetAllPersonsQuery.cs
using AutoMapper;
using Core.Domain.Filtering;
using Core.Domain.Pagination;
using Core.Domain.Repositories;
using Core.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using PersonMgmt.Application.DTOs;
using PersonMgmt.Domain.Aggregates;
using PersonMgmt.Domain.Specifications;

namespace PersonMgmt.Application.Queries;

public class GetAllPersonsQuery : IRequest<Result<PagedList<PersonResponse>>>
{
    public GetAllPersonsQuery(PagedRequest pagedRequest, string? filterString = null)
    {
        PagedRequest = pagedRequest ?? throw new ArgumentNullException(nameof(pagedRequest));
        FilterString = filterString;
    }

    public PagedRequest PagedRequest { get; set; }
    public string? FilterString { get; set; }

    // ✅ Handler nested class
    public class Handler : IRequestHandler<GetAllPersonsQuery, Result<PagedList<PersonResponse>>>
    {
        private readonly ILogger<Handler> _logger;
        private readonly IMapper _mapper;
        private readonly IRepository<Person> _personRepository;

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
                // Validation
                if (!request.PagedRequest.IsValid())
                {
                    _logger.LogWarning(
                        "Invalid pagination parameters - PageNumber: {PageNumber}, PageSize: {PageSize}",
                        request.PagedRequest.PageNumber,
                        request.PagedRequest.PageSize);
                    return Result<PagedList<PersonResponse>>.Failure("Invalid pagination parameters");
                }

                _logger.LogInformation(
                    "Fetching all persons - Filter: {FilterString}, Page: {PageNumber}, Size: {PageSize}, Sort: {SortBy} {SortDirection}",
                    request.FilterString ?? "none",
                    request.PagedRequest.PageNumber,
                    request.PagedRequest.PageSize,
                    request.PagedRequest.SortBy ?? "Default",
                    request.PagedRequest.SortDirection);

                // 1. Specification (filtering + sorting + includes)
                var spec = new GetPersonsWithFiltersSpecification(
                    request.FilterString,
                    request.PagedRequest.PageNumber,
                    request.PagedRequest.PageSize);

                // 2. Repository call (pagination from PagedRequest)
                var pagedList = await _personRepository.GetAllAsync(
                    spec,
                    request.PagedRequest,
                    cancellationToken);

                // 3. Mapping
                var responses = _mapper.Map<List<PersonResponse>>(pagedList.Data);

                // 4. Wrap in PagedList
                var result = new PagedList<PersonResponse>(
                    responses,
                    pagedList.TotalCount,
                    pagedList.PageNumber,
                    pagedList.PageSize);

                _logger.LogInformation(
                    "Retrieved persons successfully - Total: {Total}, Returned: {Returned}, Page: {Page}/{Pages}",
                    result.TotalCount,
                    responses.Count,
                    result.PageNumber,
                    result.TotalPages);

                return Result<PagedList<PersonResponse>>.Success(
                    result,
                    $"Retrieved {responses.Count} persons (page {result.PageNumber} of {result.TotalPages})");
            }
            catch (FilterParsingException ex)
            {
                _logger.LogWarning(ex, "Filter parsing error: {FilterString}", request.FilterString);
                return Result<PagedList<PersonResponse>>.Failure($"Invalid filter: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error fetching persons - Filter: {FilterString}", request.FilterString);
                return Result<PagedList<PersonResponse>>.Failure("An unexpected error occurred");
            }
        }
    }
}

// GetPersonsWithFiltersSpecification
public class GetPersonsWithFiltersSpecification : BaseFilteredSpecification<Person>
{
    public GetPersonsWithFiltersSpecification(
        string? filterString,
        int pageNumber = 1,
        int pageSize = 20)
    {
        // Includes
        AddInclude(p => p.Student);
        AddInclude(p => p.HealthRecord);
        
        // Base criteria
        AddCriteria(p => !p.IsDeleted);
        
        // Dynamic filtering
        if (!string.IsNullOrWhiteSpace(filterString))
        {
            var filterParser = new FilterParser<Person>(
                whitelist: new PersonFilterWhitelist());
            var filterExpression = filterParser.Parse(filterString);
            AddCriteria(filterExpression);
        }
        
        // Soft delete
        ApplySoftDeleteFilter();
        
        // Paging
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
        
        // Sorting
        AddOrderBy(p => p.LastName);
        AddOrderBy(p => p.FirstName);
    }
}

// API Controller
[HttpGet]
public async Task<IActionResult> GetAll(
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10,
    [FromQuery] string? filterString = null,
    CancellationToken cancellationToken = default)
{
    var query = new GetAllPersonsQuery(
        new PagedRequest { PageNumber = pageNumber, PageSize = pageSize },
        filterString);
    
    var result = await _mediator.Send(query, cancellationToken);

    if (!result.IsSuccess)
        return BadRequest(result);

    return Ok(result);
}

// Usage: GET /api/persons?pageNumber=1&pageSize=10&filterString=firstName=John;personType=Student
```

---

### Örnek 3: GetAllStudentsQuery (Filtered List)

```csharp
// PersonMgmt.Application/Queries/GetAllStudentsQuery.cs
using AutoMapper;
using Core.Domain.Filtering;
using Core.Domain.Pagination;
using Core.Domain.Repositories;
using Core.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using PersonMgmt.Application.DTOs;
using PersonMgmt.Domain.Aggregates;
using PersonMgmt.Domain.Specifications;

namespace PersonMgmt.Application.Queries;

public class GetAllStudentsQuery : IRequest<Result<PagedList<PersonResponse>>>
{
    public GetAllStudentsQuery(PagedRequest pagedRequest, string? filterString = null)
    {
        PagedRequest = pagedRequest ?? throw new ArgumentNullException(nameof(pagedRequest));
        FilterString = filterString;
    }

    public PagedRequest PagedRequest { get; set; }
    public string? FilterString { get; set; }

    // ✅ Handler nested class
    public class Handler : IRequestHandler<GetAllStudentsQuery, Result<PagedList<PersonResponse>>>
    {
        private readonly ILogger<Handler> _logger;
        private readonly IMapper _mapper;
        private readonly IRepository<Person> _personRepository;

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
            GetAllStudentsQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                if (!request.PagedRequest.IsValid())
                {
                    _logger.LogWarning(
                        "Invalid pagination: PageNumber={PageNumber}, PageSize={PageSize}",
                        request.PagedRequest.PageNumber,
                        request.PagedRequest.PageSize);
                    return Result<PagedList<PersonResponse>>.Failure("Invalid pagination parameters");
                }

                _logger.LogInformation(
                    "Fetching all students - Filter: {FilterString}, Page: {PageNumber}, Size: {PageSize}",
                    request.FilterString ?? "none",
                    request.PagedRequest.PageNumber,
                    request.PagedRequest.PageSize);

                // 1. Specification (students only + eager loading)
                var spec = new GetStudentsSpecification(
                    request.FilterString,
                    request.PagedRequest.PageNumber,
                    request.PagedRequest.PageSize);

                // 2. Repository
                var pagedList = await _personRepository.GetAllAsync(
                    spec,
                    request.PagedRequest,
                    cancellationToken);

                // 3. Mapping
                var responses = _mapper.Map<List<PersonResponse>>(pagedList.Data);

                // 4. Wrap & return
                var result = new PagedList<PersonResponse>(
                    responses,
                    pagedList.TotalCount,
                    pagedList.PageNumber,
                    pagedList.PageSize);

                _logger.LogInformation(
                    "Retrieved {Count} students (Total: {Total})",
                    responses.Count,
                    result.TotalCount);

                return Result<PagedList<PersonResponse>>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching students");
                return Result<PagedList<PersonResponse>>.Failure(ex.Message);
            }
        }
    }
}

// GetStudentsSpecification
public class GetStudentsSpecification : BaseFilteredSpecification<Person>
{
    public GetStudentsSpecification(string? filterString = null, int pageNumber = 1, int pageSize = 20)
    {
        AddInclude(p => p.Student);
        AddCriteria(p => p.Student != null && !p.Student.IsDeleted && !p.IsDeleted);
        
        if (!string.IsNullOrWhiteSpace(filterString))
        {
            var filterParser = new FilterParser<Person>(
                whitelist: new PersonFilterWhitelist());
            var filterExpression = filterParser.Parse(filterString);
            AddCriteria(filterExpression);
        }

        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
        AddOrderBy(p => p.CreatedAt);
    }
}
```

---

## ✏️ COMMAND HANDLERS - Yazma İşlemleri

### Örnek 1: CreatePersonCommand

```csharp
// PersonMgmt.Application/Commands/CreatePersonCommand.cs
using AutoMapper;
using Core.Domain.Repositories;
using Core.Domain.Results;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using PersonMgmt.Application.DTOs;
using PersonMgmt.Domain.Aggregates;
using PersonMgmt.Domain.Specifications;

namespace PersonMgmt.Application.Commands;

public class CreatePersonCommand : IRequest<Result<PersonResponse>>
{
    public CreatePersonCommand(CreatePersonRequest request)
    {
        Request = request ?? throw new ArgumentNullException(nameof(request));
    }

    public CreatePersonRequest Request { get; set; }

    // ✅ Handler nested class
    public class Handler : IRequestHandler<CreatePersonCommand, Result<PersonResponse>>
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

        public async Task<Result<PersonResponse>> Handle(
            CreatePersonCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation(
                    "Creating person: {FirstName} {LastName} - {Email}",
                    request.Request.FirstName,
                    request.Request.LastName,
                    request.Request.Email);

                // 1. Check if exists (by email)
                var existsSpec = new PersonByEmailSpecification(request.Request.Email);
                var exists = await _personRepository.ExistsAsync(existsSpec, cancellationToken);
                
                if (exists)
                {
                    _logger.LogWarning("Person with email already exists: {Email}", request.Request.Email);
                    return Result<PersonResponse>.Failure("Email already in use");
                }

                // 2. Create entity
                var personName = new PersonName(request.Request.FirstName, request.Request.LastName);
                var person = new Person(
                    personName,
                    request.Request.Email,
                    request.Request.DateOfBirth,
                    request.Request.PersonType);

                // 3. Save
                await _personRepository.AddAsync(person, cancellationToken);
                await _personRepository.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Person created successfully - ID: {PersonId}", person.Id);

                // 4. Return
                var response = _mapper.Map<PersonResponse>(person);
                return Result<PersonResponse>.Success(
                    response,
                    "Person created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating person");
                return Result<PersonResponse>.Failure("Failed to create person");
            }
        }
    }
}

// Validator
public class CreatePersonCommandValidator : AbstractValidator<CreatePersonCommand>
{
    public CreatePersonCommandValidator()
    {
        RuleFor(x => x.Request)
            .NotNull().WithMessage("Request is required");
        
        RuleFor(x => x.Request.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(100).WithMessage("First name max 100 chars");
        
        RuleFor(x => x.Request.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(100).WithMessage("Last name max 100 chars");
        
        RuleFor(x => x.Request.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Valid email required");
        
        RuleFor(x => x.Request.PersonType)
            .IsInEnum().WithMessage("Invalid person type");
    }
}

// API Controller
[HttpPost]
public async Task<IActionResult> Create(
    [FromBody] CreatePersonCommand command,
    CancellationToken cancellationToken = default)
{
    var result = await _mediator.Send(command, cancellationToken);

    if (!result.IsSuccess)
        return BadRequest(result);

    return CreatedAtAction(nameof(GetPerson), new { id = result.Data.Id }, result);
}
```

---

### Örnek 2: UpdatePersonCommand

```csharp
// PersonMgmt.Application/Commands/UpdatePersonCommand.cs
using AutoMapper;
using Core.Domain.Repositories;
using Core.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using PersonMgmt.Application.DTOs;
using PersonMgmt.Domain.Aggregates;
using PersonMgmt.Domain.Specifications;

namespace PersonMgmt.Application.Commands;

public class UpdatePersonCommand : IRequest<Result<PersonResponse>>
{
    public UpdatePersonCommand(Guid personId, UpdatePersonRequest request)
    {
        PersonId = personId;
        Request = request ?? throw new ArgumentNullException(nameof(request));
    }

    public Guid PersonId { get; set; }
    public UpdatePersonRequest Request { get; set; }

    // ✅ Handler nested class
    public class Handler : IRequestHandler<UpdatePersonCommand, Result<PersonResponse>>
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

        public async Task<Result<PersonResponse>> Handle(
            UpdatePersonCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation(
                    "Updating person - ID: {PersonId}, New Email: {Email}",
                    request.PersonId,
                    request.Request.Email);

                // 1. Fetch existing
                var spec = new PersonByIdSpecification(request.PersonId);
                var person = await _personRepository.GetAsync(spec, cancellationToken);

                if (person == null)
                {
                    _logger.LogWarning("Person not found - ID: {PersonId}", request.PersonId);
                    return Result<PersonResponse>.Failure("Person not found");
                }

                // 2. Check email uniqueness (excluding self)
                if (person.Email != request.Request.Email)
                {
                    var emailSpec = new PersonByEmailSpecification(request.Request.Email);
                    var emailExists = await _personRepository.ExistsAsync(emailSpec, cancellationToken);
                    
                    if (emailExists)
                    {
                        _logger.LogWarning("Email already in use: {Email}", request.Request.Email);
                        return Result<PersonResponse>.Failure("Email already in use");
                    }
                }

                // 3. Update
                person.UpdatePersonalInfo(
                    new PersonName(request.Request.FirstName, request.Request.LastName),
                    request.Request.Email);

                await _personRepository.UpdateAsync(person, cancellationToken);
                await _personRepository.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Person updated successfully - ID: {PersonId}", request.PersonId);

                // 4. Return
                var response = _mapper.Map<PersonResponse>(person);
                return Result<PersonResponse>.Success(response, "Person updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating person - ID: {PersonId}", request.PersonId);
                return Result<PersonResponse>.Failure("Failed to update person");
            }
        }
    }
}

// API Controller
[HttpPut("{id:guid}")]
public async Task<IActionResult> Update(
    [FromRoute] Guid id,
    [FromBody] UpdatePersonRequest request,
    CancellationToken cancellationToken = default)
{
    var command = new UpdatePersonCommand(id, request);
    var result = await _mediator.Send(command, cancellationToken);

    if (!result.IsSuccess)
        return BadRequest(result);

    return Ok(result);
}
```

---

### Örnek 3: DeletePersonCommand

```csharp
// PersonMgmt.Application/Commands/DeletePersonCommand.cs
using Core.Domain.Repositories;
using Core.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using PersonMgmt.Domain.Aggregates;

namespace PersonMgmt.Application.Commands;

public class DeletePersonCommand : IRequest<Result>
{
    public DeletePersonCommand(Guid personId)
    {
        PersonId = personId;
    }

    public Guid PersonId { get; set; }

    // ✅ Handler nested class
    public class Handler : IRequestHandler<DeletePersonCommand, Result>
    {
        private readonly IRepository<Person> _personRepository;
        private readonly ILogger<Handler> _logger;

        public Handler(
            IRepository<Person> personRepository,
            ILogger<Handler> logger)
        {
            _personRepository = personRepository ?? throw new ArgumentNullException(nameof(personRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result> Handle(
            DeletePersonCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Deleting person - ID: {PersonId}", request.PersonId);

                // 1. Fetch
                var person = await _personRepository.GetByIdAsync(request.PersonId, cancellationToken);

                if (person == null)
                {
                    _logger.LogWarning("Person not found - ID: {PersonId}", request.PersonId);
                    return Result.Failure("Person not found");
                }

                // 2. Soft delete
                person.Delete();  // Sets IsDeleted = true

                await _personRepository.UpdateAsync(person, cancellationToken);
                await _personRepository.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Person deleted successfully - ID: {PersonId}", request.PersonId);

                return Result.Success("Person deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting person - ID: {PersonId}", request.PersonId);
                return Result.Failure("Failed to delete person");
            }
        }
    }
}

// API Controller
[HttpDelete("{id:guid}")]
public async Task<IActionResult> Delete(
    [FromRoute] Guid id,
    CancellationToken cancellationToken = default)
{
    var command = new DeletePersonCommand(id);
    var result = await _mediator.Send(command, cancellationToken);

    if (!result.IsSuccess)
        return BadRequest(result);

    return NoContent();  // 204
}
```

---

## 📋 BEST PRACTICES CHECKLIST

### Query Handler'ı Oluştururken

- [ ] Query class'ı içinde nested `Handler` class'ı oluştur
- [ ] `IRequestHandler<TQuery, Result<TResponse>>` implement et
- [ ] Constructor'da dependencies inject et (null checks)
- [ ] `Handle` metodu içinde try-catch yap
- [ ] Validation (PagedRequest.IsValid() vb.)
- [ ] Specification oluştur ve kullan
- [ ] Repository'den veri çek
- [ ] AutoMapper ile DTO'ya dönüştür
- [ ] Logging ekle (Info, Warning, Error)
- [ ] Result<T>.Success/Failure dön

### Command Handler'ı Oluştururken

- [ ] Command class'ı içinde nested `Handler` class'ı oluştur
- [ ] `IRequestHandler<TCommand, Result<T>>` implement et
- [ ] Constructor'da dependencies inject et
- [ ] `Handle` metodu içinde try-catch yap
- [ ] Entity existence check yap
- [ ] Business logic validation
- [ ] Entity update/create/delete yap
- [ ] `SaveChangesAsync()` çağır
- [ ] Soft delete kullan (entity.Delete(), not repository.DeleteAsync)
- [ ] Logging ekle (bilgilendirici ve audit)
- [ ] Result wrap'le

### Validator Oluştururken

- [ ] `AbstractValidator<TCommand>` inherit et
- [ ] Constructor'da `RuleFor()` ile validation yazıcı
- [ ] NotEmpty, EmailAddress, IsInEnum vb. validators
- [ ] `.WithMessage()` ile custom mesajlar

### API Controller'ı Oluştururken

- [ ] `[ApiController]` attribute
- [ ] `[Route("api/[controller]")]` attribute
- [ ] `[HttpGet/Post/Put/Delete]` attribute
- [ ] `[FromRoute]` / `[FromQuery]` / `[FromBody]` belirt
- [ ] `CancellationToken cancellationToken = default` param
- [ ] `_mediator.Send(query/command, cancellationToken)`
- [ ] `result.IsSuccess` check et
- [ ] Appropriate HTTP status return
- [ ] `CreatedAtAction()` (POST), `NoContent()` (DELETE)

---

## 🔀 QUERY vs COMMAND - ÖZET

| Aspect | Query | Command |
|--------|-------|---------|
| **Amaç** | Veri oku | Veri değiştir |
| **Dönüş** | Result<T> | Result veya Result<T> |
| **Side Effect** | YOK | VAR (SaveChangesAsync) |
| **Cache** | Yapılabilir | Yapılmaz |
| **Exception** | Hata dön (Failure) | Hata dön (Failure) |
| **Logging** | Info level | Info + Audit logging |
| **Specification** | Okuma specs | Existence checks |
| **Pagination** | PagedList dön | Tek entity dön |
| **Handler Nested** | Query class içinde | Command class içinde |

---

## 📁 Dosya Organizasyonu

```
src/Modules/PersonMgmt/Application/
├── Queries/
│   ├── GetPersonQuery.cs              # Query + nested Handler
│   ├── GetAllPersonsQuery.cs          # Query + nested Handler
│   └── GetAllStudentsQuery.cs         # Query + nested Handler
│
├── Commands/
│   ├── CreatePersonCommand.cs         # Command + nested Handler + Validator
│   ├── UpdatePersonCommand.cs         # Command + nested Handler + Validator
│   └── DeletePersonCommand.cs         # Command + nested Handler
│
├── DTOs/
│   ├── PersonRequest.cs
│   ├── PersonResponse.cs
│   └── CreatePersonRequest.cs
│
├── Validators/
│   ├── CreatePersonCommandValidator.cs
│   └── UpdatePersonCommandValidator.cs
│
└── Mappers/
    └── PersonMappingProfile.cs
```

---

## ✅ Yeni Query/Command Oluştururken Template

```csharp
// Query Template
public class Get[Entity]Query : IRequest<Result<[Entity]Response>>
{
    public Get[Entity]Query([params])
    {
        // Initialize properties
    }

    public [Entity]Id { get; set; }

    public class Handler : IRequestHandler<Get[Entity]Query, Result<[Entity]Response>>
    {
        private readonly IRepository<[Entity]> _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<Handler> _logger;

        public Handler(IRepository<[Entity]> repository, IMapper mapper, ILogger<Handler> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<[Entity]Response>> Handle(Get[Entity]Query request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching [Entity]...");
                
                var spec = new Get[Entity]Specification([params]);
                var entity = await _repository.GetAsync(spec, cancellationToken);
                
                if (entity == null)
                    return Result<[Entity]Response>.Failure("[Entity] not found");
                
                var response = _mapper.Map<[Entity]Response>(entity);
                return Result<[Entity]Response>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching [Entity]");
                return Result<[Entity]Response>.Failure(ex.Message);
            }
        }
    }
}
```

---

**Next:** Kod repository'sinde real-world örnekleri incele → Test et → Deploy!