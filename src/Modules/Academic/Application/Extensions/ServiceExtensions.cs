using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using AutoMapper;
using Academic.Application.Validators;
using Academic.Application.Mappers;
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