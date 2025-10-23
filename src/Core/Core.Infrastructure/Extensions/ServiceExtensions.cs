using Core.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PersonMgmt.Domain.Interfaces;

namespace Core.Infrastructure.Extensions;

/// <summary>
/// Infrastructure Layer DI Registration
/// 
/// Sorumluluğu:
/// - Database context'i register etmek
/// - Repository'leri register etmek
/// - Infrastructure services'leri register etmek
/// 
/// Kullanım (Program.cs'de):
/// services.AddInfrastructure(configuration);
/// </summary>
public static class ServiceExtensions
{
    /// <summary>
    /// Infrastructure services'leri DI container'a ekle
    /// </summary>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ==================== DATABASE ====================

        // DbContext registration
        services.AddDbContext<AppDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException(
                    "Connection string 'DefaultConnection' not found in configuration.");
            }

            options.UseSqlServer(
                connectionString,
                sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly("Infrastructure");
                    sqlOptions.EnableRetryOnFailure(maxRetryCount: 3, maxRetryDelay: TimeSpan.FromSeconds(10), errorNumbersToAdd: null);
                });

            if (IsEnvironmentDevelopment())
            {
                options.EnableSensitiveDataLogging(true);
            }
        });

        // ==================== REPOSITORIES ====================

        // PersonMgmt Repositories
        services.AddScoped<IPersonRepository, PersonRepository>();

        // TODO: Diğer modüller için repositories eklenecek
        // services.AddScoped<IAcademicRepository, AcademicRepository>();
        // services.AddScoped<IVirtualPOSRepository, VirtualPOSRepository>();
        // vb.

        // ==================== UNIT OF WORK ====================

        // UnitOfWork pattern (isteğe bağlı)
        // services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }

    /// <summary>
    /// Environment kontrolü (development mi?)
    /// </summary>
    private static bool IsEnvironmentDevelopment()
    {
        return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
    }
}