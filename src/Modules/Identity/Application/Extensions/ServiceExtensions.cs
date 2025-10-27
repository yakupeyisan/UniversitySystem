using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Application.Extensions;

/// <summary>
/// Dependency injection extension methods for Identity.Application
/// </summary>
public static class ServiceExtensions
{
    /// <summary>
    /// Adds Identity application layer services to the dependency injection container
    /// </summary>
    public static IServiceCollection AddIdentityApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining(typeof(ServiceExtensions)));
        services.AddValidatorsFromAssemblyContaining(typeof(ServiceExtensions));
        services.AddAutoMapper(config =>
        {
            config.AddProfile<IdentityMappingProfile>();
        });
        return services;
    }


    /// <summary>
    /// Adds Identity domain services to the dependency injection container
    /// This should be called from the Infrastructure or API layer
    /// </summary>
    public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure JWT options
        services.Configure<JwtOptions>(options =>
            configuration.GetSection("Jwt").Bind(options));

        return services;
    }
}