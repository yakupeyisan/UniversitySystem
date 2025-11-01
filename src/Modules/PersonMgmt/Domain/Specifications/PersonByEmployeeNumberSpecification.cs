using Core.Domain.Specifications;
using PersonMgmt.Domain.Aggregates;
namespace PersonMgmt.Domain.Specifications;
public class PersonByEmployeeNumberSpecification : Specification<Person>
{
    public PersonByEmployeeNumberSpecification(string employeeNumber)
    {
        AddInclude(p => p.Staff);
        Criteria = p => p.IsDeleted != true && p.Staff != null && p.Staff.EmployeeNumber == employeeNumber;
    }
    public PersonByEmployeeNumberSpecification(Guid excludeId, string employeeNumber)
    {
        AddInclude(p => p.Staff);
        Criteria = p =>
            p.Id != excludeId && p.IsDeleted != true && p.Staff != null && p.Staff.EmployeeNumber == employeeNumber;
    }
}