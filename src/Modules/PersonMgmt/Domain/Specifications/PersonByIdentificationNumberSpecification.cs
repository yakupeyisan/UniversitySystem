using Core.Domain.Specifications;
using PersonMgmt.Domain.Aggregates;
namespace PersonMgmt.Domain.Specifications;
public class PersonByIdentificationNumberSpecification : Specification<Person>
{
    public PersonByIdentificationNumberSpecification(string identificationNumber)
    {
        Criteria = p => p.IsDeleted != true && p.IdentificationNumber == identificationNumber;
    }
    public PersonByIdentificationNumberSpecification(Guid excludeId, string identificationNumber)
    {
        Criteria = p => p.Id != excludeId && p.IsDeleted != true && p.IdentificationNumber == identificationNumber;
    }
}