using Core.Domain.Specifications;
using PersonMgmt.Domain.Aggregates;

namespace PersonMgmt.Domain.Specifications;

public class PersonByEmailSpecification : Specification<Person>
{
    public PersonByEmailSpecification(string email)
    {
        Criteria = p => p.IsDeleted != true && p.Email == email;
    }

    public PersonByEmailSpecification(Guid excludeId, string email)
    {
        Criteria = p => p.Id != excludeId && p.IsDeleted != true && p.Email == email;
    }
}