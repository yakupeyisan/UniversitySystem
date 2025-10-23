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
        // Temel bilgiler
        "email",
        "phonenumber",
        "name",
        "gender",
        "birthdate",
        "nationalid",
        "departmentid",
        "student",
        "staff",
        
        // Timestamp
        "createdat",
        "updatedat",
        "isdeleted"
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