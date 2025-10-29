using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using PersonMgmt.Application.Mappers;

namespace PersonMgmt.Application.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddPersonMgmtApplication(
        this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining(typeof(ServiceExtensions)));
        services.AddValidatorsFromAssemblyContaining(typeof(ServiceExtensions));
        services.AddAutoMapper(cfg => { }, typeof(PersonMgmtMappingProfile));
        return services;
    }
}