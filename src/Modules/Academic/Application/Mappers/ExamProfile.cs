using Academic.Application.DTOs;
using Academic.Domain.Aggregates;
using AutoMapper;

namespace Academic.Application.Mappers;

public class ExamProfile : Profile
{
    public ExamProfile()
    {
        CreateMap<Exam, ExamResponse>()
            .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course!.Name))
            .ForMember(dest => dest.ExamType, opt => opt.MapFrom(src => src.ExamType.ToString()))
            .ForMember(dest => dest.ExamDate, opt => opt.MapFrom(src => src.ExamDate.ToString("yyyy-MM-dd")))
            .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.TimeSlot.StartTime.ToString("HH:mm")))
            .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.TimeSlot.EndTime.ToString("HH:mm")))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
    }
}