using AutoMapper;

namespace Core.Application.Common.Mappings;

/// <summary>
/// MappingProfile - AutoMapper Profile base class
/// 
/// Sorumluluğu:
/// - AutoMapper mapping configurations provide etme
/// - Her module'ün kendi mapping profile'ı olur
/// 
/// Kullanım (Module'de):
/// public class PersonMgmtMappingProfile : MappingProfile
/// {
///     public PersonMgmtMappingProfile()
///     {
///         CreateMap<Person, PersonResponse>();
///         CreateMap<CreatePersonRequest, Person>();
///         // ...
///     }
/// }
/// 
/// Sonra Program.cs'de:
/// services.AddAutoMapper(typeof(PersonMgmtMappingProfile));
/// 
/// Note:
/// - Her module'ün kendi mapping profile'ı vardır
/// - Core.Application'da generic mapping'ler olabilir
/// </summary>
public abstract class MappingProfile : Profile
{
    /// <summary>
    /// Constructor - derived class'lar mapping'leri configure et
    /// </summary>
    protected MappingProfile()
    {
        // Derived class'lar ctor'da CreateMap() çağırır
    }
}