using AutoMapper;
using PersonMgmt.Application.DTOs;
using PersonMgmt.Domain.Aggregates;
using PersonMgmt.Domain.Enums;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PersonMgmt.Application.Mappers;

/// <summary>
/// PersonMgmt AutoMapper profile
/// 
/// Mapping kurallarını tanımlar:
/// - Person -> PersonResponse
/// - CreatePersonRequest -> Person
/// - Enums -> String
/// </summary>
public class PersonMgmtMappingProfile : Profile
{
    /// <summary>
    /// Constructor
    /// </summary>
    public PersonMgmtMappingProfile()
    {
        // Person -> PersonResponse
        CreateMap<Person, PersonResponse>()
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Name.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Name.LastName))
            .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => ConvertGenderToString(src.Gender)))
            .ForMember(dest => dest.IsStudent, opt => opt.MapFrom(src => src.Student != null))
            .ForMember(dest => dest.IsStaff, opt => opt.MapFrom(src => src.Staff != null))
            .ForMember(dest => dest.ActiveRestrictionCount, opt => opt.MapFrom(src => src.GetActiveRestrictions().Count()));

        // CreatePersonRequest -> Person
        CreateMap<CreatePersonRequest, Person>();

        // UpdatePersonRequest -> Person (partial)
        CreateMap<UpdatePersonRequest, Person>();

        // Gender Enum -> String
        CreateMap<Gender, string>().ConvertUsing(src => src.ToString());
    }

    /// <summary>
    /// Gender enum'unu Türkçe string'e çevir
    /// </summary>
    private static string ConvertGenderToString(Gender gender) => gender switch
    {
        Gender.Male => "Erkek",
        Gender.Female => "Kadın",
        _ => "Bilinmiyor"
    };
}