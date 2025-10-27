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
        // Service implementations will be registered in Infrastructure layer
        // Example:
        // services.AddScoped<ITokenService, TokenService>();
        // services.AddScoped<IPasswordHasher, PasswordHasher>();
        // services.AddScoped<IEmailService, EmailService>();

        // Configure JWT options
        services.Configure<JwtOptions>(options =>
            configuration.GetSection("Jwt").Bind(options));

        return services;
    }
}

/// <summary>
/// JWT configuration options
/// </summary>
public class JwtOptions
{
    /// <summary>
    /// Secret key for signing JWT tokens
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Issuer of the JWT token
    /// </summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// Audience of the JWT token
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// Access token expiration time in minutes (default: 60)
    /// </summary>
    public int AccessTokenExpirationMinutes { get; set; } = 60;

    /// <summary>
    /// Refresh token expiration time in days (default: 7)
    /// </summary>
    public int RefreshTokenExpirationDays { get; set; } = 7;
}