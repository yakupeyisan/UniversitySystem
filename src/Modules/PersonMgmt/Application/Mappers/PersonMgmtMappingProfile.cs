using AutoMapper;
using PersonMgmt.Application.DTOs;
using PersonMgmt.Domain.Aggregates;
using PersonMgmt.Domain.Enums;

namespace PersonMgmt.Application.Mappers;

/// <summary>
/// AutoMapper Profile for PersonMgmt Module
/// Handles all Entity-to-DTO and DTO-to-Entity mappings
/// Including enum conversions for Turkish language support
/// </summary>
public class PersonMgmtMappingProfile : Profile
{
    public PersonMgmtMappingProfile()
    {
        // ============================================
        // PERSON AGGREGATE MAPPINGS
        // ============================================

        /// <summary>
        /// Maps Person aggregate to PersonResponse DTO
        /// Extracts name from ValueObject and calculates derived properties
        /// </summary>
        CreateMap<Person, PersonResponse>()
            .ForMember(dest => dest.Id,
                opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.FirstName,
                opt => opt.MapFrom(src => src.Name.FirstName))
            .ForMember(dest => dest.LastName,
                opt => opt.MapFrom(src => src.Name.LastName))
            .ForMember(dest => dest.IdentificationNumber,
                opt => opt.MapFrom(src => src.IdentificationNumber))
            .ForMember(dest => dest.BirthDate,
                opt => opt.MapFrom(src => src.BirthDate))
            .ForMember(dest => dest.Gender,
                opt => opt.MapFrom(src => ConvertGenderToString(src.Gender)))
            .ForMember(dest => dest.Email,
                opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.PhoneNumber,
                opt => opt.MapFrom(src => src.PhoneNumber))
            .ForMember(dest => dest.DepartmentId,
                opt => opt.MapFrom(src => src.DepartmentId))
            .ForMember(dest => dest.ProfilePhotoUrl,
                opt => opt.MapFrom(src => src.ProfilePhotoUrl))
            .ForMember(dest => dest.IsStudent,
                opt => opt.MapFrom(src => src.Student != null && !src.Student.IsDeleted))
            .ForMember(dest => dest.IsStaff,
                opt => opt.MapFrom(src => src.Staff != null && !src.Staff.IsDeleted))
            .ForMember(dest => dest.ActiveRestrictionCount,
                opt => opt.MapFrom(src => src.GetActiveRestrictions().Count()))
            .ForMember(dest => dest.CreatedAt,
                opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt,
                opt => opt.MapFrom(src => src.UpdatedAt));

        /// <summary>
        /// Maps CreatePersonRequest DTO to Person aggregate
        /// Ignores system-generated fields and relationships
        /// </summary>
        CreateMap<CreatePersonRequest, Person>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Name, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Student, opt => opt.Ignore())
            .ForMember(dest => dest.Staff, opt => opt.Ignore())
            .ForMember(dest => dest.HealthRecord, opt => opt.Ignore())
            .ForMember(dest => dest.Addresses, opt => opt.Ignore())
            .ForMember(dest => dest.Restrictions, opt => opt.Ignore());

        /// <summary>
        /// Maps UpdatePersonRequest DTO to Person aggregate
        /// Only maps non-null values - conditional mapping for optional fields
        /// </summary>
        CreateMap<UpdatePersonRequest, Person>()
            .ForMember(dest => dest.Email,
                opt => opt.Condition(src => !string.IsNullOrEmpty(src.Email)))
            .ForMember(dest => dest.PhoneNumber,
                opt => opt.Condition(src => !string.IsNullOrEmpty(src.PhoneNumber)))
            .ForMember(dest => dest.DepartmentId,
                opt => opt.Condition(src => src.DepartmentId.HasValue))
            .ForMember(dest => dest.ProfilePhotoUrl,
                opt => opt.Condition(src => !string.IsNullOrEmpty(src.ProfilePhotoUrl)))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Name, opt => opt.Ignore())
            .ForMember(dest => dest.IdentificationNumber, opt => opt.Ignore())
            .ForMember(dest => dest.BirthDate, opt => opt.Ignore())
            .ForMember(dest => dest.Gender, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Student, opt => opt.Ignore())
            .ForMember(dest => dest.Staff, opt => opt.Ignore())
            .ForMember(dest => dest.HealthRecord, opt => opt.Ignore())
            .ForMember(dest => dest.Addresses, opt => opt.Ignore())
            .ForMember(dest => dest.Restrictions, opt => opt.Ignore());

        // ============================================
        // STUDENT ENTITY MAPPINGS
        // ============================================

        /// <summary>
        /// Maps Student entity to StudentResponse DTO
        /// Converts enums to Turkish string representations
        /// Converts CGPA float to decimal for API response
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
        // STAFF ENTITY MAPPINGS
        // ============================================

        /// <summary>
        /// Maps Staff entity to StaffResponse DTO
        /// Converts AcademicTitle enum to Turkish string
        /// IsActive boolean to Turkish employment status text
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
                opt => opt.Ignore()) // NOTE: DepartmentId should be mapped from Person entity separately
            .ForMember(dest => dest.Salary,
                opt => opt.Ignore()) // NOTE: Salary not modeled in current entities
            .ForMember(dest => dest.EmploymentStatus,
                opt => opt.MapFrom(src => src.IsActive ? "Aktif" : "Pasif"))
            .ForMember(dest => dest.CreatedAt,
                opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt,
                opt => opt.MapFrom(src => src.UpdatedAt));

        // ============================================
        // HEALTH RECORD MAPPINGS
        // ============================================

        /// <summary>
        /// Maps HealthRecord entity to HealthRecordResponse DTO
        /// Handles all nullable health information fields
        /// </summary>
        CreateMap<HealthRecord, HealthRecordResponse>()
            .ForMember(dest => dest.Id,
                opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.PersonId,
                opt => opt.MapFrom(src => src.PersonId))
            .ForMember(dest => dest.BloodType,
                opt => opt.MapFrom(src => src.BloodType ?? string.Empty))
            .ForMember(dest => dest.Allergies,
                opt => opt.MapFrom(src => src.Allergies ?? string.Empty))
            .ForMember(dest => dest.ChronicDiseases,
                opt => opt.MapFrom(src => src.ChronicDiseases ?? string.Empty))
            .ForMember(dest => dest.Medications,
                opt => opt.MapFrom(src => src.Medications ?? string.Empty))
            .ForMember(dest => dest.EmergencyHealthInfo,
                opt => opt.MapFrom(src => src.EmergencyHealthInfo ?? string.Empty))
            .ForMember(dest => dest.Notes,
                opt => opt.MapFrom(src => src.Notes ?? string.Empty))
            .ForMember(dest => dest.LastCheckupDate,
                opt => opt.MapFrom(src => src.LastCheckupDate))
            .ForMember(dest => dest.CreatedAt,
                opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt,
                opt => opt.MapFrom(src => src.UpdatedAt));

        // ============================================
        // PERSON RESTRICTION MAPPINGS
        // ============================================

        /// <summary>
        /// Maps PersonRestriction entity to RestrictionResponse DTO
        /// Converts enum values to Turkish string representations
        /// Checks both IsActive property and date range for IsCurrentlyActive
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
        // ENUM TO STRING CONVERSIONS
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
    /// Used for API responses
    /// </summary>
    private static string ConvertGenderToString(Gender gender) => gender switch
    {
        Gender.Male => "Erkek",
        Gender.Female => "Kadın",
        _ => "Bilinmiyor"
    };

    /// <summary>
    /// Converts EducationLevel enum to Turkish string representation
    /// Values: Bachelor (Lisans), Master (Yüksek Lisans), Doctorate (Doktora)
    /// </summary>
    private static string ConvertEducationLevelToString(EducationLevel level) => level switch
    {
        EducationLevel.Associate => "Ön Lisans",
        EducationLevel.Bachelor => "Lisans",
        EducationLevel.Master => "Yüksek Lisans",
        EducationLevel.PhD => "Doktora",
        _ => "Bilinmiyor"
    };

    /// <summary>
    /// Converts StudentStatus enum to Turkish string representation
    /// Values: Active, OnLeave, Suspended, Passive, Graduated, Expelled
    /// </summary>
    private static string ConvertStudentStatusToString(StudentStatus status) => status switch
    {
        StudentStatus.Active => "Aktif",
        StudentStatus.OnLeave => "İzinde",
        StudentStatus.Suspended => "Askıya Alınmış",
        StudentStatus.Passive => "Pasif",
        StudentStatus.Graduated => "Mezun",
        StudentStatus.Expelled => "İhraç Edilmiş",
        _ => "Bilinmiyor"
    };

    /// <summary>
    /// Converts AcademicTitle enum to Turkish string representation
    /// Used for Staff position/title information
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
    /// Describes WHY the restriction was applied (reason/type)
    /// Values: Suspended, Banned, CovidQuarantine, CriminalRecord, MedicalReason, Financial, Other
    /// </summary>
    private static string ConvertRestrictionTypeToString(RestrictionType type) => type switch
    {
        RestrictionType.Suspended => "Askıya Alınmış",
        RestrictionType.Banned => "Yasaklanmış",
        RestrictionType.CovidQuarantine => "Covid Karantinası",
        RestrictionType.CriminalRecord => "Ceza Sicili",
        RestrictionType.MedicalReason => "Tıbbi Neden",
        RestrictionType.Financial => "Mali",
        RestrictionType.Other => "Diğer",
        _ => "Bilinmiyor"
    };

    /// <summary>
    /// Converts RestrictionLevel enum to Turkish string representation
    /// Describes WHERE/SCOPE the restriction applies to
    /// Values: General (Kurum Genelinde), Cafeteria, AllFacilities, Specific
    /// </summary>
    private static string ConvertRestrictionLevelToString(RestrictionLevel level) => level switch
    {
        RestrictionLevel.General => "Kurum Genelinde",
        RestrictionLevel.Cafeteria => "Yemekhanede",
        RestrictionLevel.AllFacilities => "Tüm Tesislerde",
        RestrictionLevel.Specific => "Belirli Alanlarda",
        _ => "Bilinmiyor"
    };
}