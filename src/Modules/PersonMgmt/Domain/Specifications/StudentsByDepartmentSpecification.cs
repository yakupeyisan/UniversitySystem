using Core.Domain.Specifications;
using PersonMgmt.Domain.Aggregates;
using PersonMgmt.Domain.Enums;
namespace PersonMgmt.Domain.Specifications;
public class StudentsByDepartmentSpecification : Specification<Person>
{
    public StudentsByDepartmentSpecification(Guid departmentId)
    {
        Criteria = p => !p.IsDeleted &&
                        p.DepartmentId == departmentId &&
                        p.Student != null &&
                        p.Student.Status == StudentStatus.Active;
        AddOrderBy(p => p.Name);
    }
}