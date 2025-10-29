using Academic.Application.Mappers;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Academic.Application.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddAcademicApplication(
        this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining(typeof(ServiceExtensions)));
        services.AddValidatorsFromAssemblyContaining(typeof(ServiceExtensions));
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