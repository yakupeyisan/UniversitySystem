using Core.Domain.Filtering;

namespace PersonMgmt.Domain.Filtering;

public class PersonFilterWhitelist : IFilterWhitelist
{
    private static readonly HashSet<string> AllowedProperties = new(StringComparer.OrdinalIgnoreCase)
    {
        "name.firstName",
        "name.lastName",
        "name.fullName",
        "email",
        "phoneNumber",
        "gender",
        "birthDate",
        "departmentId",
        "createdAt",
        "updatedAt",
        "isDeleted",
        "student.studentNumber",
        "student.status",
        "student.educationLevel",
        "student.currentSemester",
        "student.cgpa",
        "student.sgpa",
        "student.enrollmentDate",
        "student.graduationDate",
        "staff.employeeNumber",
        "staff.academicTitle",
        "staff.hireDate",
        "staff.isActive",
        "staff.address.street",
        "staff.address.city",
        "staff.address.country",
        "healthRecord.bloodType",
        "healthRecord.allergies",
        "healthRecord.chronicDiseases",
        "healthRecord.lastCheckupDate",
        "restrictions.restrictionType",
        "restrictions.restrictionLevel",
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