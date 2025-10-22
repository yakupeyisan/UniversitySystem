using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using PersonMgmt.Application.Mappers;
using PersonMgmt.Application.Validators;

namespace PersonMgmt.Application.Extensions;

/// <summary>
/// PersonMgmt Application layer DI registration
/// 
/// Kullanım (Program.cs'de):
/// services.AddPersonMgmtApplication();
/// 
/// Bu method aşağıdaki servisleri register eder:
/// - MediatR (Commands/Queries handlers)
/// - FluentValidation (Validators)
/// - AutoMapper (DTOs mapping)
/// </summary>
public static class ServiceExtensions
{
    /// <summary>
    /// PersonMgmt.Application servisleri ekle
    /// </summary>
    public static IServiceCollection AddPersonMgmtApplication(
        this IServiceCollection services)
    {
        // MediatR - Commands ve Queries handlers
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining(typeof(ServiceExtensions)));

        // FluentValidation - Validators
        services.AddValidatorsFromAssemblyContaining(typeof(ServiceExtensions));

        // AutoMapper
        services.AddAutoMapper(cfg => { }, typeof(PersonMgmtMappingProfile));

        // Behavior'lar (MediatR Pipeline)
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}