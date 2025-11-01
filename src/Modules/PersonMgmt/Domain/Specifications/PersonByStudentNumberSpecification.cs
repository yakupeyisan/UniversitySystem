using Core.Domain.Specifications;
using PersonMgmt.Domain.Aggregates;
namespace PersonMgmt.Domain.Specifications;
public class PersonByStudentNumberSpecification : Specification<Person>
{
    public PersonByStudentNumberSpecification(string studentNumber)
    {
        AddInclude(p => p.Student);
        Criteria = p => p.IsDeleted != true && p.Student != null && p.Student.StudentNumber == studentNumber;
    }
    public PersonByStudentNumberSpecification(Guid excludeId, string studentNumber)
    {
        AddInclude(p => p.Student);
        Criteria = p =>
            p.Id != excludeId && p.IsDeleted != true && p.Student != null && p.Student.StudentNumber == studentNumber;
    }
}