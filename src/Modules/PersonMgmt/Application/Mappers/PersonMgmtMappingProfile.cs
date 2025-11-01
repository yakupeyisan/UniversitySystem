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
                opt => opt.Ignore())
            .ForMember(dest => dest.Salary,
                opt => opt.Ignore())
            .ForMember(dest => dest.EmploymentStatus,
                opt => opt.MapFrom(src => src.IsActive ? "Aktif" : "Pasif"))
            .ForMember(dest => dest.CreatedAt,
                opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt,
                opt => opt.MapFrom(src => src.UpdatedAt));
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
        CreateMap<Gender, string>().ConvertUsing(src => ConvertGenderToString(src));
        CreateMap<EducationLevel, string>().ConvertUsing(src => ConvertEducationLevelToString(src));
        CreateMap<StudentStatus, string>().ConvertUsing(src => ConvertStudentStatusToString(src));
        CreateMap<AcademicTitle, string>().ConvertUsing(src => ConvertAcademicTitleToString(src));
        CreateMap<RestrictionType, string>().ConvertUsing(src => ConvertRestrictionTypeToString(src));
        CreateMap<RestrictionLevel, string>().ConvertUsing(src => ConvertRestrictionLevelToString(src));
    }
    private static string ConvertGenderToString(Gender gender)
    {
        return gender switch
        {
            Gender.Male => "Erkek",
            Gender.Female => "Kadın",
            _ => "Bilinmiyor"
        };
    }
    private static string ConvertEducationLevelToString(EducationLevel level)
    {
        return level switch
        {
            EducationLevel.Associate => "Ön Lisans",
            EducationLevel.Bachelor => "Lisans",
            EducationLevel.Master => "Yüksek Lisans",
            EducationLevel.PhD => "Doktora",
            _ => "Bilinmiyor"
        };
    }
    private static string ConvertStudentStatusToString(StudentStatus status)
    {
        return status switch
        {
            StudentStatus.Active => "Aktif",
            StudentStatus.OnLeave => "İzinde",
            StudentStatus.Suspended => "Askıya Alınmış",
            StudentStatus.Passive => "Pasif",
            StudentStatus.Graduated => "Mezun",
            StudentStatus.Expelled => "İhraç Edilmiş",
            _ => "Bilinmiyor"
        };
    }
    private static string ConvertAcademicTitleToString(AcademicTitle title)
    {
        return title switch
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
    }
    private static string ConvertRestrictionTypeToString(RestrictionType type)
    {
        return type switch
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
    }
    private static string ConvertRestrictionLevelToString(RestrictionLevel level)
    {
        return level switch
        {
            RestrictionLevel.General => "Kurum Genelinde",
            RestrictionLevel.Cafeteria => "Yemekhanede",
            RestrictionLevel.AllFacilities => "Tüm Tesislerde",
            RestrictionLevel.Specific => "Belirli Alanlarda",
            _ => "Bilinmiyor"
        };
    }
}