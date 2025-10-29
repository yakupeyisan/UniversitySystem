using Core.Domain.Specifications;
using PersonMgmt.Domain.Aggregates;

namespace PersonMgmt.Domain.Specifications;

public class ActivePersonsSpecification : Specification<Person>
{
    public ActivePersonsSpecification()
    {
        Criteria = p => !p.IsDeleted;
        AddOrderBy(p => p.Name);
    }
}