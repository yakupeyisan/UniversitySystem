using Core.Domain.Filtering;

namespace PersonMgmt.Domain.Filtering;

/// <summary>
/// Person entity'si için filter whitelist
/// 
/// Kullanım:
/// - Security: Sadece belirli properties'e filter izni ver
/// - Hassas alanlar filtre edilemez (SSN, internal flags, vs)
/// 
/// İzin verilen alanlar:
/// ✅ Email, PhoneNumber, Name, Gender
/// ✅ BirthDate, NationalId
/// ✅ DepartmentId
/// ❌ ProfilePhotoUrl, internal flags
/// 
/// Örnek:
/// "email|contains|@example.com;gender|eq|Male;birthDate|gte|1990-01-01"
/// </summary>
public class PersonFilterWhitelist : IFilterWhitelist
{
    /// <summary>
    /// Filtrelenebilir alanlar (case-insensitive)
    /// </summary>
    private static readonly HashSet<string> AllowedProperties = new(StringComparer.OrdinalIgnoreCase)
    {
        // ==================== BASIC PROPERTIES ====================
        
        // Ad & Soyad (ValueObject property'leri)
        "name.firstName",      // PersonName.FirstName
        "name.lastName",       // PersonName.LastName
        "name.fullName",       // PersonName.FullName (computed)

        // Contact Information
        "email",
        "phoneNumber",

        // Personal Info
        "gender",              // Gender enum
        "birthDate",

        // Department
        "departmentId",

        // Metadata
        "createdAt",
        "updatedAt",
        "isDeleted",

        // ==================== STUDENT-SPECIFIC ====================
        
        "student.studentNumber",
        "student.status",           // StudentStatus enum
        "student.educationLevel",   // EducationLevel enum
        "student.currentSemester",
        "student.cgpa",
        "student.sgpa",
        "student.enrollmentDate",
        "student.graduationDate",

        // ==================== STAFF-SPECIFIC ====================
        
        "staff.employeeNumber",
        "staff.academicTitle",       // AcademicTitle enum
        "staff.hireDate",
        "staff.isActive",
        "staff.address.street",      // Address ValueObject properties
        "staff.address.city",
        "staff.address.country",

        // ==================== HEALTH RECORD ====================
        
        "healthRecord.bloodType",
        "healthRecord.allergies",
        "healthRecord.chronicDiseases",
        "healthRecord.lastCheckupDate",

        // ==================== RESTRICTIONS ====================
        
        "restrictions.restrictionType",   // RestrictionType enum
        "restrictions.restrictionLevel",  // RestrictionLevel enum
        "restrictions.isActive",
        "restrictions.startDate",
        "restrictions.endDate",
        "restrictions.severity"
    };

    public bool IsAllowed(string propertyName)
    {
        if (string.IsNullOrWhiteSpace(propertyName))
            return false;

        return AllowedProperties.Contains(propertyName);
    }

    IReadOnlySet<string> IFilterWhitelist.GetAllowedProperties()
    {
        return AllowedProperties;
    }

}