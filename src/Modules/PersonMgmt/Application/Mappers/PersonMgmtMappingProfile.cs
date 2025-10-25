using AutoMapper;
using PersonMgmt.Application.DTOs;
using PersonMgmt.Domain.Aggregates;
using PersonMgmt.Domain.Enums;
namespace PersonMgmt.Application.Mappers;
public class PersonMgmtMappingProfile : Profile
{
    public PersonMgmtMappingProfile()
    {
        CreateMap<Person, PersonResponse>()
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Name.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Name.LastName))
            .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => ConvertGenderToString(src.Gender)))
            .ForMember(dest => dest.IsStudent, opt => opt.MapFrom(src => src.Student != null))
            .ForMember(dest => dest.IsStaff, opt => opt.MapFrom(src => src.Staff != null))
            .ForMember(dest => dest.ActiveRestrictionCount, opt => opt.MapFrom(src => src.GetActiveRestrictions().Count()));
        CreateMap<CreatePersonRequest, Person>();
        CreateMap<UpdatePersonRequest, Person>();
        CreateMap<Gender, string>().ConvertUsing(src => src.ToString());
    }
    private static string ConvertGenderToString(Gender gender) => gender switch
    {
        Gender.Male => "Erkek",
        Gender.Female => "Kadın",
        _ => "Bilinmiyor"
    };
}