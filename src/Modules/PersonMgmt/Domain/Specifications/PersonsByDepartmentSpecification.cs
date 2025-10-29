using Core.Domain.Specifications;
using PersonMgmt.Domain.Aggregates;

namespace PersonMgmt.Domain.Specifications;

public class PersonsByDepartmentSpecification : Specification<Person>
{
    public PersonsByDepartmentSpecification(Guid departmentId)
    {
        Criteria = p => !p.IsDeleted && p.DepartmentId == departmentId;
        AddOrderBy(p => p.Name);
    }
}