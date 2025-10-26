using Academic.Application.DTOs;
using Academic.Domain.Aggregates;
using Academic.Domain.Enums;
using AutoMapper;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Academic.Application.Mappers;

public class CourseProfile : Profile
{
    public CourseProfile()
    {
        // Course → Response
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

        // Request → Domain
        CreateMap<CreateCourseRequest, Course>()
            .ForMember(dest => dest.Level, opt => opt.MapFrom(src => (CourseLevel)src.Level))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => (CourseType)src.Type))
            .ForMember(dest => dest.Semester, opt => opt.MapFrom(src => (CourseSemester)src.Semester));
    }
}

public class GradeProfile : Profile
{
    public GradeProfile()
    {
        // Grade → Response
        CreateMap<Grade, GradeResponse>()
            .ForMember(dest => dest.LetterGrade, opt => opt.MapFrom(src => src.LetterGrade.ToString()));

        // GradeObjection → Response
        CreateMap<GradeObjection, GradeObjectionResponse>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.NewLetterGrade, opt => opt.MapFrom(src => src.NewLetterGrade.HasValue ? src.NewLetterGrade.Value.ToString() : null));
    }
}

public class EnrollmentProfile : Profile
{
    public EnrollmentProfile()
    {
        // CourseRegistration → Response
        CreateMap<CourseRegistration, CourseRegistrationResponse>()
            .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course!.Name))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        // CourseWaitingListEntry → Response
        CreateMap<CourseWaitingListEntry, WaitingListResponse>()
            .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course!.Name))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
    }
}

public class ExamProfile : Profile
{
    public ExamProfile()
    {
        // Exam → Response
        CreateMap<Exam, ExamResponse>()
            .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course!.Name))
            .ForMember(dest => dest.ExamType, opt => opt.MapFrom(src => src.ExamType.ToString()))
            .ForMember(dest => dest.ExamDate, opt => opt.MapFrom(src => src.ExamDate.ToString("yyyy-MM-dd")))
            .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.TimeSlot.StartTime.ToString("HH:mm")))
            .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.TimeSlot.EndTime.ToString("HH:mm")))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
    }
}
