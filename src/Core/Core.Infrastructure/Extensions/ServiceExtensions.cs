using Core.Infrastructure.Persistence.Contexts;
using Core.Infrastructure.Persistence.Repositories.PersonMgmt;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PersonMgmt.Domain.Interfaces;
namespace Core.Infrastructure.Extensions;
public static class ServiceExtensions
{
    public static IServiceCollection AddInfrastructure(
    this IServiceCollection services,
    IConfiguration configuration)
    {
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
                    sqlOptions.MigrationsAssembly("Core.Infrastructure");
                    sqlOptions.EnableRetryOnFailure(maxRetryCount: 3, maxRetryDelay: TimeSpan.FromSeconds(10), errorNumbersToAdd: null);
                });
            if (IsEnvironmentDevelopment())
            {
                options.EnableSensitiveDataLogging(true);
            }
        });
        services.AddScoped<IPersonRepository, PersonRepository>();
        return services;
    }
    private static bool IsEnvironmentDevelopment()
    {
        return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
    }
}