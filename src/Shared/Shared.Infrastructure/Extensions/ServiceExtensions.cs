using Academic.Domain.Interfaces;
using Identity.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PersonMgmt.Domain.Interfaces;
using Shared.Infrastructure.Persistence.Contexts;
using Shared.Infrastructure.Persistence.Repositories.Academic;
using Shared.Infrastructure.Persistence.Repositories.Identity;
using Shared.Infrastructure.Persistence.Repositories.PersonMgmt;
namespace Shared.Infrastructure.Extensions;
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
                    sqlOptions.MigrationsAssembly("Shared.Infrastructure");
                    sqlOptions.EnableRetryOnFailure(maxRetryCount: 3, maxRetryDelay: TimeSpan.FromSeconds(10), errorNumbersToAdd: null);
                });
            if (IsEnvironmentDevelopment())
            {
                options.EnableSensitiveDataLogging(true);
            }
        });
        services.AddScoped<IPersonRepository, PersonRepository>();
        services.AddScoped<ICourseRepository, CourseRepository>();
        services.AddScoped<IWaitingListRepository, WaitingListRepository>();
        services.AddScoped<IExamRepository, ExamRepository>();
        services.AddScoped<IGradeRepository, GradeRepository>();
        services.AddScoped<IGradeObjectionRepository, GradeObjectionRepository>();
        services.AddScoped<ICourseRegistrationRepository, CourseRegistrationRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        return services;
    }
    private static bool IsEnvironmentDevelopment()
    {
        return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
    }
}