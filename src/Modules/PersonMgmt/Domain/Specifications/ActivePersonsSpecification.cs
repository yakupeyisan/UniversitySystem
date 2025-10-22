using Core.Domain.Specifications;
using PersonMgmt.Domain.Aggregates;

namespace PersonMgmt.Domain.Specifications;

/// <summary>
/// Specification - Aktif kişileri getir
/// 
/// Kullanım:
/// var spec = new ActivePersonsSpecification();
/// var persons = await _repository.GetAsync(spec);
/// </summary>
public class ActivePersonsSpecification : Specification<Person>
{
    public ActivePersonsSpecification()
    {
        // Soft deleted olmayan kişiler
        Criteria = p => !p.IsDeleted;

        // Sırayla
        AddOrderBy(p => p.Name);
    }
}