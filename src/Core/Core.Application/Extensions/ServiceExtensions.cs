using Core.Application.Behaviors;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Application.Extensions;

/// <summary>
/// ServiceExtensions - Core.Application DI registration
/// 
/// Sorumluluğu:
/// - Core.Application'ın tüm servisleri register etme
/// - MediatR, FluentValidation, AutoMapper yapılandırması
/// 
/// Kullanım (Program.cs):
/// services.AddCoreApplication();
/// 
/// Bu method aşağıdaki servisleri register eder:
/// 1. MediatR - Tüm Command/Query handlers
/// 2. FluentValidation - Tüm validators
/// 3. ValidationBehavior - MediatR pipeline'a validation ekleme
/// 4. Diğer Core.Application services
/// 
/// DI sırası:
/// 1. AddCoreApplication() ← YOU ARE HERE
/// 2. AddCoreInfrastructure()
/// 3. AddPersonMgmtApplication()
/// 4. AddPersonMgmtInfrastructure()
/// 5. ... (other modules)
/// 6. MapEndpoints() (API)
/// </summary>
public static class ServiceExtensions
{
    /// <summary>
    /// Core.Application servisleri register et
    /// </summary>
    public static IServiceCollection AddCoreApplication(
        this IServiceCollection services)
    {
        // MediatR - Command/Query handlers
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(ServiceExtensions).Assembly);
        });

        // FluentValidation - Validators
        services.AddValidatorsFromAssembly(typeof(ServiceExtensions).Assembly);

        // MediatR Pipeline Behaviors
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}