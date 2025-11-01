using Core.Domain.Specifications;
using PersonMgmt.Domain.Aggregates;
namespace PersonMgmt.Domain.Specifications;
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