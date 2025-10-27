using AutoMapper;
using Identity.Application.DTOs;
using Identity.Domain.Aggregates;

public class IdentityMappingProfile : Profile
{
    public IdentityMappingProfile()
    {
        // User mappings
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.Roles))
            .ForMember(dest => dest.Permissions, opt => opt.MapFrom(src => src.Permissions))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));

        // Role mappings
        CreateMap<Role, RoleDto>()
            .ForMember(dest => dest.RoleType, opt => opt.MapFrom(src => src.RoleType.ToString()))
            .ForMember(dest => dest.Permissions, opt => opt.MapFrom(src => src.Permissions));

        // Permission mappings
        CreateMap<Permission, PermissionDto>()
            .ForMember(dest => dest.PermissionType, opt => opt.MapFrom(src => src.PermissionType.ToString()));

        // RefreshToken mappings
        CreateMap<RefreshToken, RefreshTokenDto>();
    }
}