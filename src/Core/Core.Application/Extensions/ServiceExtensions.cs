using Core.Application.Behaviors;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
namespace Core.Application.Extensions;
public static class ServiceExtensions
{
    public static IServiceCollection AddCoreApplication(
    this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(ServiceExtensions).Assembly);
        });
        services.AddValidatorsFromAssembly(typeof(ServiceExtensions).Assembly);
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        return services;
    }
}