using Academic.Application.DTOs;
using Academic.Domain.Aggregates;
using AutoMapper;
namespace Academic.Application.Mappers;
public class GradeProfile : Profile
{
    public GradeProfile()
    {
        CreateMap<Grade, GradeResponse>()
            .ForMember(dest => dest.LetterGrade, opt => opt.MapFrom(src => src.LetterGrade.ToString()));
        CreateMap<GradeObjection, GradeObjectionResponse>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.NewLetterGrade, opt => opt.MapFrom(src => src.NewLetterGrade.HasValue ? src.NewLetterGrade.Value.ToString() : null));
    }
}