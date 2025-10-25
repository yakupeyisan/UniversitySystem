using Core.Domain.Specifications;
using PersonMgmt.Domain.Aggregates;
using PersonMgmt.Domain.Enums;
namespace PersonMgmt.Domain.Specifications;
public class PersonsByDepartmentSpecification : Specification<Person>
{
    public PersonsByDepartmentSpecification(Guid departmentId)
    {
        Criteria = p => !p.IsDeleted && p.DepartmentId == departmentId;
        AddOrderBy(p => p.Name);
    }
}

public class PersonsByNameSearchSpecification : Specification<Person>
{
    public PersonsByNameSearchSpecification(string firstName, string lastName)
    {
        Criteria = p => !p.IsDeleted &&
                        (p.Name.FirstName.Contains(firstName) ||
                         p.Name.LastName.Contains(lastName));

        AddOrderBy(p => p.Name.FirstName);
        AddOrderBy(p => p.Name.LastName);
    }
}
public class StaffByPositionSpecification : Specification<Person>
{
    public StaffByPositionSpecification(string position)
    {
        if (Enum.TryParse<AcademicTitle>(position, ignoreCase: true, out var academicTitle))
        {
            Criteria = p => !p.IsDeleted &&
                            p.Staff != null &&
                            p.Staff.IsActive &&
                            p.Staff.AcademicTitle == academicTitle;
        }
        else
        {
            Criteria = p => false;
        }
        AddOrderBy(p => p.Name.FirstName);
    }
}

public class StudentsByProgramSpecification : Specification<Person>
{
    public StudentsByProgramSpecification(Guid programId)
    {
        Criteria = p => !p.IsDeleted &&
                        p.Student != null &&
                        p.Student.ProgramId == programId &&
                        p.Student.Status == StudentStatus.Active;

        AddOrderBy(p => p.Name.FirstName);
    }
}
public class StudentsByGpaRangeSpecification : Specification<Person>
{
    public StudentsByGpaRangeSpecification(double minGpa, double maxGpa)
    {
        Criteria = p => !p.IsDeleted &&
                        p.Student != null &&
                        p.Student.CGPA >= minGpa &&
                        p.Student.CGPA <= maxGpa &&
                        p.Student.Status == StudentStatus.Active;

        AddOrderByDescending(p => p.Student.CGPA);
    }
}










