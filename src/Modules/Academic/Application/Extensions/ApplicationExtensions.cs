using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using AutoMapper;
using Academic.Application.Validators;
using Academic.Application.Mappers;

namespace Academic.Application.Extensions;

/// <summary>
/// Service collection extensions for Academic module registration
/// </summary>
public static class ServiceExtensions
{
    /// <summary>
    /// Add Academic module services to the dependency injection container
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddAcademicApplicationServices(
        this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining(typeof(ServiceExtensions)));
        services.AddValidatorsFromAssemblyContaining(typeof(ServiceExtensions));

        // AutoMapper
        services.AddAutoMapper(config =>
        {
            config.AddProfile<CourseProfile>();
            config.AddProfile<GradeProfile>();
            config.AddProfile<EnrollmentProfile>();
            config.AddProfile<ExamProfile>();
        });


        return services;
    }
}