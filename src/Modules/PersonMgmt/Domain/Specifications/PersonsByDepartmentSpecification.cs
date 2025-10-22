using Core.Domain.Specifications;
using PersonMgmt.Domain.Aggregates;

namespace PersonMgmt.Domain.Specifications;

/// <summary>
/// Specification - Departmana göre kişileri getir
/// 
/// Kullanım:
/// var spec = new PersonsByDepartmentSpecification(departmentId);
/// var persons = await _repository.GetAsync(spec);
/// </summary>
public class PersonsByDepartmentSpecification : Specification<Person>
{
    public PersonsByDepartmentSpecification(Guid departmentId)
    {
        Criteria = p => !p.IsDeleted && p.DepartmentId == departmentId;
        AddOrderBy(p => p.Name);
    }
}