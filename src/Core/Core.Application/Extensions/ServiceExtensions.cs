using Core.Application.Abstractions;
using Core.Application.Behaviors;
using Core.Application.Services;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
namespace Core.Application.Extensions;
public static class ServiceExtensions
{
    public static IServiceCollection AddCoreApplication(
    this IServiceCollection services)
    {
        services.AddScoped<IDateTime, DateTimeService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(ServiceExtensions).Assembly);
        });
        services.AddValidatorsFromAssembly(typeof(ServiceExtensions).Assembly);
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        return services;
    }
}