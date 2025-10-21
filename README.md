UniversitySystem - Clean Architecture Project Structure
📁 Genel Dosya Yapısı
UniversitySystem/
├── src/
│   ├── Core/
│   │   ├── Core.Domain/
│   │   ├── Core.Application/
│   │   └── Core.Infrastructure/
│   │
│   ├── Modules/
│   │   ├── VirtualPOS/
│   │   │   ├── Domain/
│   │   │   ├── Application/
│   │   │   ├── Infrastructure/
│   │   │   └── API/
│   │   ├── Academic/
│   │   ├── AccessControl/
│   │   ├── Cafeteria/
│   │   ├── EventTicketing/
│   │   ├── Library/
│   │   ├── Parking/
│   │   ├── PersonMgmt/
│   │   ├── Payroll/
│   │   ├── Research/
│   │   └── Wallet/
│   │
│   └── Presentation/
│       ├── API/
│       │   ├── Controllers/
│       │   ├── Middlewares/
│       │   ├── Extensions/
│       │   └── Program.cs
│       └── Web/ (Frontend - Responsive)
│
├── tests/
│   ├── Unit/
│   └── Integration/
│
├── docs/
│   ├── Architecture.md
│   ├── API.md
│   └── UniversitySystem.erdiagram
│
└── UniversitySystem.sln
🏗️ Clean Architecture Katmanları
1. Domain Layer (Core İş Mantığı)
Module.Domain/
├── Aggregates/
│   ├── [AggregateRoot].cs
│   └── Entities/
├── ValueObjects/
├── DomainEvents/
├── Enums/
├── Exceptions/
└── Interfaces/
    └── IRepository.cs
Sorumluluklar:

İş kurallarını tanımlama
Domain Events
Value Objects ve Entities
Repository interfaces

2. Application Layer (Use Cases)
Module.Application/
├── Commands/
│   ├── Create[Entity]Command.cs
│   ├── Update[Entity]Command.cs
│   └── Handlers/
├── Queries/
│   ├── Get[Entity]Query.cs
│   └── Handlers/
├── DTOs/
│   ├── Request/
│   └── Response/
├── Validators/
├── Mappers/
├── Services/
└── Exceptions/
Sorumluluklar:

CQRS Commands & Queries
Business Logic Orchestration
Data Transfer Objects
Validation

3. Infrastructure Layer (Teknik Implementasyon)
Module.Infrastructure/
├── Persistence/
│   ├── Repositories/
│   ├── DbContext.cs
│   ├── Migrations/
│   └── Configurations/
├── Services/
│   ├── ExternalAPIs/
│   └── Messaging/
├── Events/
│   └── EventHandlers/
└── Configuration/
Sorumluluklar:

Database operasyonları
External services
Event Publishing
UnitOfWork pattern

4. Presentation Layer (API)
API/
├── Controllers/
│   ├── VirtualPOSController.cs
│   ├── AcademicController.cs
│   └── [ModuleController].cs
├── Middlewares/
│   ├── ErrorHandlingMiddleware.cs
│   ├── AuthenticationMiddleware.cs
│   └── LoggingMiddleware.cs
├── Extensions/
│   └── ServiceCollectionExtensions.cs
└── Program.cs
Sorumluluklar:

HTTP endpoints
Request/Response handling
Authentication & Authorization
Error handling


🎯 SOLID Prensipleri Uygulaması
Single Responsibility Principle (SRP)

Her sınıf bir sorumluluğa sahip
Repository: Veri erişim
Service: İş mantığı
Validator: Doğrulama

Open/Closed Principle (OCP)

Extension için açık, modification için kapalı
Interface-based design
Strategy pattern kullanımı

Liskov Substitution Principle (LSP)

Derived sınıflar base type yerine kullanılabilir
IRepository implementations

Interface Segregation Principle (ISP)

Küçük, spesifik interfaces
Clients sadece ihtiyaç duyduğu metodları implement eder

Dependency Inversion Principle (DIP)

High-level modules abstract interfaces'e depend eder
Constructor injection


🔄 CQRS Pattern Implementasyonu
Command (Yazma İşlemleri)
csharppublic class CreatePaymentCommand : ICommand<PaymentDto>
{
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; }
}

public class CreatePaymentCommandHandler : ICommandHandler<CreatePaymentCommand, PaymentDto>
{
    public async Task<PaymentDto> Handle(CreatePaymentCommand command, CancellationToken cancellationToken)
    {
        // Business Logic
    }
}
Query (Okuma İşlemleri)
csharppublic class GetPaymentByIdQuery : IQuery<PaymentDto>
{
    public Guid PaymentId { get; set; }
}

public class GetPaymentByIdQueryHandler : IQueryHandler<GetPaymentByIdQuery, PaymentDto>
{
    public async Task<PaymentDto> Handle(GetPaymentByIdQuery query, CancellationToken cancellationToken)
    {
        // Read-only logic
    }
}

📦 Repository & Unit of Work Pattern
Repository Interface (ISP)
csharppublic interface IPaymentRepository : IGenericRepository<Payment>
{
    Task<Payment> GetByTransactionIdAsync(string transactionId);
    Task<IEnumerable<Payment>> GetByStatusAsync(PaymentStatus status);
}

public interface IGenericRepository<T> where T : IEntity
{
    Task<T> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
}
Unit of Work
csharppublic interface IUnitOfWork : IAsyncDisposable
{
    IPaymentRepository Payments { get; }
    ITransactionRepository Transactions { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<bool> BeginTransactionAsync();
    Task<bool> CommitAsync();
    Task<bool> RollbackAsync();
}

🎪 Dependency Injection Yapılandırması
csharppublic static class ServiceCollectionExtensions
{
    public static IServiceCollection AddVirtualPOSModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Domain Services
        services.AddScoped<IPaymentService, PaymentService>();
        
        // Application Services
        services.AddScoped<IPaymentApplicationService, PaymentApplicationService>();
        
        // Repositories
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        // MediatR Commands & Queries
        services.AddMediatR(typeof(CreatePaymentCommandHandler));
        
        // Validators
        services.AddValidatorsFromAssemblyContaining<CreatePaymentCommandValidator>();
        
        // AutoMapper
        services.AddAutoMapper(typeof(PaymentProfile));
        
        return services;
    }
}

🔊 Event-Driven Architecture
Domain Event
csharppublic class PaymentProcessedDomainEvent : DomainEvent
{
    public Guid PaymentId { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; }
}
Event Handler
csharppublic class PaymentProcessedEventHandler : INotificationHandler<PaymentProcessedDomainEvent>
{
    public async Task Handle(PaymentProcessedDomainEvent notification, CancellationToken cancellationToken)
    {
        // Müşteri para yüklendi, e-posta gönder
        // Wallet'ı güncelle
        // Diğer modülleri notify et
    }
}
Event Publishing
csharppublic class PaymentAggregate : AggregateRoot
{
    public void ProcessPayment(decimal amount)
    {
        // Business Logic
        
        // Event Raise
        AddDomainEvent(new PaymentProcessedDomainEvent
        {
            PaymentId = this.Id,
            Amount = amount,
            Status = "Completed"
        });
    }
}

🛡️ Exception Handling
Custom Exceptions (ISP)
csharppublic abstract class ApplicationException : Exception
{
    public abstract int StatusCode { get; }
    public abstract string ErrorCode { get; }
}

public class PaymentNotFoundException : ApplicationException
{
    public override int StatusCode => StatusCodes.Status404NotFound;
    public override string ErrorCode => "PAYMENT_NOT_FOUND";
}

public class InsufficientBalanceException : ApplicationException
{
    public override int StatusCode => StatusCodes.Status400BadRequest;
    public override string ErrorCode => "INSUFFICIENT_BALANCE";
}
Global Exception Middleware
csharppublic class ErrorHandlingMiddleware
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        var response = exception switch
        {
            ApplicationException appEx => new ErrorResponse
            {
                StatusCode = appEx.StatusCode,
                ErrorCode = appEx.ErrorCode,
                Message = appEx.Message
            },
            _ => new ErrorResponse
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                ErrorCode = "INTERNAL_SERVER_ERROR",
                Message = "An unexpected error occurred"
            }
        };
        
        context.Response.StatusCode = response.StatusCode;
        return context.Response.WriteAsJsonAsync(response);
    }
}

📱 Responsive Design (Frontend)
API Response Standardı
csharppublic class ApiResponse<T>
{
    public bool Success { get; set; }
    public T Data { get; set; }
    public string Message { get; set; }
    public DateTime Timestamp { get; set; }
}
Mobile-Friendly Endpoints
csharp[ApiController]
[Route("api/v1/[controller]")]
public class PaymentsController : ControllerBase
{
    [HttpGet("{id}")]
    [ProduceResponseType(typeof(ApiResponse<PaymentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPayment(Guid id)
    {
        var query = new GetPaymentByIdQuery { PaymentId = id };
        var result = await _mediator.Send(query);
        return Ok(new ApiResponse<PaymentDto> { Data = result, Success = true });
    }
}

🧪 Testing Strategy
Unit Test (Domain + Application)
csharppublic class PaymentApplicationServiceTests
{
    [Fact]
    public async Task CreatePayment_ValidInput_ReturnsPaymentDto()
    {
        // Arrange
        var command = new CreatePaymentCommand { Amount = 100 };
        var handler = new CreatePaymentCommandHandler(_mockRepository, _mockUnitOfWork);
        
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.Should().NotBeNull();
        result.Amount.Should().Be(100);
    }
}
Integration Test
csharppublic class PaymentIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task CreatePayment_ValidRequest_Returns201Created()
    {
        // Arrange
        var client = _factory.CreateClient();
        var command = new CreatePaymentCommand { Amount = 100 };
        
        // Act
        var response = await client.PostAsJsonAsync("/api/v1/payments", command);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}

🔐 Authentication & Authorization
JWT Implementation
csharppublic class JwtService : IJwtService
{
    public string GenerateToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim("role", user.Role)
        };
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

📊 Logging & Monitoring
Serilog Configuration
csharpLog.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/app-.txt", rollingInterval: RollingInterval.Day)
    .WriteTo.Seq("http://localhost:5341")
    .Enrich.FromLogContext()
    .CreateLogger();

🚀 CI/CD Integration
GitHub Actions / Azure Pipelines

Automated testing
Code coverage
Deployment automation
Performance monitoring


📋 Checklist

 Domain Layer: Aggregates, Entities, Value Objects
 Application Layer: Commands, Queries, Validators
 Infrastructure Layer: Repositories, DbContext, Migrations
 Presentation Layer: Controllers, Middlewares
 DI Configuration
 Error Handling
 Authentication & Authorization
 Logging & Monitoring
 Unit Tests
 Integration Tests
 API Documentation (Swagger)
 Responsive UI