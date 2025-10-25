using AutoMapper;
using PersonMgmt.Application.DTOs;
using PersonMgmt.Domain.Aggregates;
using PersonMgmt.Domain.Enums;
namespace PersonMgmt.Application.Mappers;

public class PersonMgmtMappingProfile : Profile
{
    public PersonMgmtMappingProfile()
    {
        // ============================================
        // PERSON MAPPINGS
        // ============================================

        /// <summary>
        /// Maps Person aggregate to PersonResponse DTO
        /// Includes student/staff status and active restrictions count
        /// </summary>
        CreateMap<Person, PersonResponse>()
            .ForMember(dest => dest.FirstName,
                opt => opt.MapFrom(src => src.Name.FirstName))
            .ForMember(dest => dest.LastName,
                opt => opt.MapFrom(src => src.Name.LastName))
            .ForMember(dest => dest.Gender,
                opt => opt.MapFrom(src => ConvertGenderToString(src.Gender)))
            .ForMember(dest => dest.IsStudent,
                opt => opt.MapFrom(src => src.Student != null && !src.Student.IsDeleted))
            .ForMember(dest => dest.IsStaff,
                opt => opt.MapFrom(src => src.Staff != null && !src.Staff.IsDeleted))
            .ForMember(dest => dest.ActiveRestrictionCount,
                opt => opt.MapFrom(src => src.GetActiveRestrictions().Count()))
            .ReverseMap()
            .ForMember(dest => dest.Name, opt => opt.Ignore()) // ✅ ValueObject'i ignore et
            .ForMember(dest => dest.Student, opt => opt.Ignore())
            .ForMember(dest => dest.Staff, opt => opt.Ignore())
            .ForMember(dest => dest.Restrictions, opt => opt.Ignore());

        /// <summary>
        /// Maps CreatePersonRequest DTO to Person aggregate
        /// Used in CreatePersonCommand
        /// </summary>
        CreateMap<CreatePersonRequest, Person>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Name, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore());

        /// <summary>
        /// Maps UpdatePersonRequest DTO to Person aggregate
        /// Used in UpdatePersonCommand - Null fields are ignored
        /// </summary>
        CreateMap<UpdatePersonRequest, Person>()
            .ForMember(dest => dest.Email,
                opt => opt.Condition(src => !string.IsNullOrEmpty(src.Email)))
            .ForMember(dest => dest.PhoneNumber,
                opt => opt.Condition(src => !string.IsNullOrEmpty(src.PhoneNumber)))
            .ForMember(dest => dest.DepartmentId,
                opt => opt.Condition(src => src.DepartmentId.HasValue))
            .ForMember(dest => dest.ProfilePhotoUrl,
                opt => opt.Condition(src => src.ProfilePhotoUrl != null))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Name, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Student, opt => opt.Ignore())
            .ForMember(dest => dest.Staff, opt => opt.Ignore())
            .ForMember(dest => dest.Restrictions, opt => opt.Ignore());

        // ============================================
        // STUDENT MAPPINGS
        // ============================================

        /// <summary>
        /// Maps Student entity to StudentResponse DTO
        /// Converts enum values to strings for API response
        /// </summary>
        CreateMap<Student, StudentResponse>()
            .ForMember(dest => dest.PersonId,
                opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.StudentNumber,
                opt => opt.MapFrom(src => src.StudentNumber))
            .ForMember(dest => dest.ProgramId,
                opt => opt.MapFrom(src => src.ProgramId ?? Guid.Empty))
            .ForMember(dest => dest.EnrollmentDate,
                opt => opt.MapFrom(src => src.EnrollmentDate))
            .ForMember(dest => dest.EducationLevel,
                opt => opt.MapFrom(src => ConvertEducationLevelToString(src.EducationLevel)))
            .ForMember(dest => dest.StudentStatus,
                opt => opt.MapFrom(src => ConvertStudentStatusToString(src.Status)))
            .ForMember(dest => dest.GPA,
                opt => opt.MapFrom(src => (decimal)src.CGPA))
            .ForMember(dest => dest.CreatedAt,
                opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt,
                opt => opt.MapFrom(src => src.UpdatedAt));

        // ============================================
        // STAFF MAPPINGS
        // ============================================

        /// <summary>
        /// Maps Staff entity to StaffResponse DTO
        /// Converts enum values and calculates derived fields
        /// Note: DepartmentId and Salary come from Person entity relationship
        /// </summary>
        CreateMap<Staff, StaffResponse>()
            .ForMember(dest => dest.PersonId,
                opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.EmployeeNumber,
                opt => opt.MapFrom(src => src.EmployeeNumber))
            .ForMember(dest => dest.Position,
                opt => opt.MapFrom(src => ConvertAcademicTitleToString(src.AcademicTitle)))
            .ForMember(dest => dest.HireDate,
                opt => opt.MapFrom(src => src.HireDate))
            .ForMember(dest => dest.DepartmentId,
                opt => opt.Ignore()) // ⚠️ DepartmentId Person'dan alınmalı
            .ForMember(dest => dest.Salary,
                opt => opt.Ignore()) // ⚠️ Salary entities'de tanımlanmış değil
            .ForMember(dest => dest.EmploymentStatus,
                opt => opt.MapFrom(src => src.IsActive ? "Aktif" : "Pasif"))
            .ForMember(dest => dest.CreatedAt,
                opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt,
                opt => opt.MapFrom(src => src.UpdatedAt));

        // ============================================
        // RESTRICTION MAPPINGS
        // ============================================

        /// <summary>
        /// Maps PersonRestriction entity to RestrictionResponse DTO
        /// Converts enum values to strings for API response
        /// </summary>
        CreateMap<PersonRestriction, RestrictionResponse>()
            .ForMember(dest => dest.Id,
                opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.PersonId,
                opt => opt.MapFrom(src => src.PersonId))
            .ForMember(dest => dest.RestrictionType,
                opt => opt.MapFrom(src => ConvertRestrictionTypeToString(src.RestrictionType)))
            .ForMember(dest => dest.RestrictionLevel,
                opt => opt.MapFrom(src => ConvertRestrictionLevelToString(src.RestrictionLevel)))
            .ForMember(dest => dest.StartDate,
                opt => opt.MapFrom(src => src.StartDate))
            .ForMember(dest => dest.EndDate,
                opt => opt.MapFrom(src => src.EndDate))
            .ForMember(dest => dest.Reason,
                opt => opt.MapFrom(src => src.Reason))
            .ForMember(dest => dest.Severity,
                opt => opt.MapFrom(src => src.Severity))
            .ForMember(dest => dest.IsActive,
                opt => opt.MapFrom(src => src.IsActive && src.IsCurrentlyActive()))
            .ForMember(dest => dest.AppliedBy,
                opt => opt.MapFrom(src => src.AppliedBy))
            .ForMember(dest => dest.CreatedAt,
                opt => opt.MapFrom(src => src.CreatedAt));

        // ============================================
        // ENUM CONVERSIONS
        // ============================================

        CreateMap<Gender, string>().ConvertUsing(src => ConvertGenderToString(src));
        CreateMap<EducationLevel, string>().ConvertUsing(src => ConvertEducationLevelToString(src));
        CreateMap<StudentStatus, string>().ConvertUsing(src => ConvertStudentStatusToString(src));
        CreateMap<AcademicTitle, string>().ConvertUsing(src => ConvertAcademicTitleToString(src));
        CreateMap<RestrictionType, string>().ConvertUsing(src => ConvertRestrictionTypeToString(src));
        CreateMap<RestrictionLevel, string>().ConvertUsing(src => ConvertRestrictionLevelToString(src));
    }

    // ============================================
    // HELPER CONVERSION METHODS
    // ============================================

    /// <summary>
    /// Converts Gender enum to Turkish string representation
    /// </summary>
    private static string ConvertGenderToString(Gender gender) => gender switch
    {
        Gender.Male => "Erkek",
        Gender.Female => "Kadın",
        _ => "Bilinmiyor"
    };

    /// <summary>
    /// Converts EducationLevel enum to Turkish string representation
    /// </summary>
    private static string ConvertEducationLevelToString(EducationLevel level) => level switch
    {
        EducationLevel.Bachelor => "Lisans",
        EducationLevel.Master => "Yüksek Lisans",
        EducationLevel.Doctorate => "Doktora",
        _ => "Bilinmiyor"
    };

    /// <summary>
    /// Converts StudentStatus enum to Turkish string representation
    /// </summary>
    private static string ConvertStudentStatusToString(StudentStatus status) => status switch
    {
        StudentStatus.Active => "Aktif",
        StudentStatus.Suspended => "Askıya Alınmış",
        StudentStatus.Passive => "Pasif",
        StudentStatus.Graduated => "Mezun",
        StudentStatus.Expelled => "İhraç Edilmiş",
        _ => "Bilinmiyor"
    };

    /// <summary>
    /// Converts AcademicTitle enum to Turkish string representation
    /// </summary>
    private static string ConvertAcademicTitleToString(AcademicTitle title) => title switch
    {
        AcademicTitle.Assistant => "Asistan",
        AcademicTitle.ResearchAssistant => "Araştırma Görevlisi",
        AcademicTitle.Lecturer => "Öğretim Görevlisi",
        AcademicTitle.AssociateProfessor => "Doçent",
        AcademicTitle.Professor => "Profesör",
        AcademicTitle.Doctor => "Doktor",
        AcademicTitle.Unknown => "Bilinmiyor",
        _ => "Bilinmiyor"
    };

    /// <summary>
    /// Converts RestrictionType enum to Turkish string representation
    /// </summary>
    private static string ConvertRestrictionTypeToString(RestrictionType type) => type switch
    {
        RestrictionType.Academic => "Akademik",
        RestrictionType.Disciplinary => "Disiplin",
        RestrictionType.Financial => "Mali",
        RestrictionType.Administrative => "İdari",
        RestrictionType.Library => "Kütüphane",
        RestrictionType.Dormitory => "Yurt",
        RestrictionType.Other => "Diğer",
        _ => "Bilinmiyor"
    };

    /// <summary>
    /// Converts RestrictionLevel enum to Turkish string representation
    /// </summary>
    private static string ConvertRestrictionLevelToString(RestrictionLevel level) => level switch
    {
        RestrictionLevel.Warning => "Uyarı",
        RestrictionLevel.Caution => "İhtiyat",
        RestrictionLevel.Serious => "Ciddi",
        RestrictionLevel.Critical => "Kritik",
        _ => "Bilinmiyor"
    };
}