using Academic.Application.DTOs;
using Academic.Domain.Aggregates;
using Academic.Domain.Enums;
using AutoMapper;
namespace Academic.Application.Mappers;
public class CourseProfile : Profile
{
    public CourseProfile()
    {
        CreateMap<Course, CourseResponse>()
            .ForMember(dest => dest.Level, opt => opt.MapFrom(src => src.Level.ToString()))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
            .ForMember(dest => dest.Semester, opt => opt.MapFrom(src => src.Semester.ToString()))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.MaxCapacity, opt => opt.MapFrom(src => src.Capacity.MaxCapacity))
            .ForMember(dest => dest.CurrentEnrollment, opt => opt.MapFrom(src => src.Capacity.CurrentEnrollment))
            .ForMember(dest => dest.OccupancyPercentage, opt => opt.MapFrom(src => src.Capacity.OccupancyPercentage()))
            .ForMember(dest => dest.InstructorCount, opt => opt.MapFrom(src => src.InstructorIds.Count))
            .ForMember(dest => dest.PrerequisiteCount, opt => opt.MapFrom(src => src.PrerequisiteIds.Count));
        CreateMap<Course, CourseListResponse>()
            .ForMember(dest => dest.Semester, opt => opt.MapFrom(src => src.Semester.ToString()))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.MaxCapacity, opt => opt.MapFrom(src => src.Capacity.MaxCapacity))
            .ForMember(dest => dest.CurrentEnrollment, opt => opt.MapFrom(src => src.Capacity.CurrentEnrollment));
        CreateMap<CreateCourseRequest, Course>()
            .ForMember(dest => dest.Level, opt => opt.MapFrom(src => (CourseLevel)src.Level))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => (CourseType)src.Type))
            .ForMember(dest => dest.Semester, opt => opt.MapFrom(src => (CourseSemester)src.Semester));
    }
}