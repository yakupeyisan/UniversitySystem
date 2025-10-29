using Academic.Application.DTOs;
using Academic.Domain.Aggregates;
using AutoMapper;

namespace Academic.Application.Mappers;

public class EnrollmentProfile : Profile
{
    public EnrollmentProfile()
    {
        CreateMap<CourseRegistration, CourseRegistrationResponse>()
            .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course!.Name))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
        CreateMap<CourseWaitingListEntry, WaitingListResponse>()
            .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course!.Name))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
    }
}