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

        // TwoFactorToken mappings
        CreateMap<TwoFactorToken, TwoFactorStatusDto>()
            .ForMember(dest => dest.IsEnabled, opt => opt.MapFrom(src => src.IsActive && src.IsVerified))
            .ForMember(dest => dest.Method, opt => opt.MapFrom(src => src.Method.ToString()))
            .ForMember(dest => dest.EnabledAt, opt => opt.MapFrom(src => src.VerifiedAt))
            .ForMember(dest => dest.RemainingBackupCodes,
                opt => opt.MapFrom(src => src.GetBackupCodes().Count));

        // LoginHistory mappings
        CreateMap<LoginHistory, LoginHistoryDto>()
            .ForMember(dest => dest.Result, opt => opt.MapFrom(src => src.Result.ToString()));

        // FailedLoginAttempt mappings
        CreateMap<FailedLoginAttempt, FailedLoginAttemptDto>();

        // UserAccountLockout mappings
        CreateMap<UserAccountLockout, AccountLockoutStatusDto>()
            .ForMember(dest => dest.LockReason, opt => opt.MapFrom(src => src.Reason.ToString()));
    }
}