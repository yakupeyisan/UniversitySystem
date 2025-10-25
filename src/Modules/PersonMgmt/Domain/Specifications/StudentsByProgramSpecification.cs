using Core.Domain.Specifications;
using PersonMgmt.Domain.Aggregates;
using PersonMgmt.Domain.Enums;
namespace PersonMgmt.Domain.Specifications;
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