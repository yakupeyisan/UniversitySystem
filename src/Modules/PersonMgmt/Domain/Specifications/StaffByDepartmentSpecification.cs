using Core.Domain.Specifications;
using PersonMgmt.Domain.Aggregates;
namespace PersonMgmt.Domain.Specifications;
public class StaffByDepartmentSpecification : Specification<Person>
{
    public StaffByDepartmentSpecification(Guid departmentId)
    {
        Criteria = p => !p.IsDeleted &&
                        p.DepartmentId == departmentId &&
                        p.Staff != null &&
                        p.Staff.IsActive;
        AddOrderBy(p => p.Name);
    }
}