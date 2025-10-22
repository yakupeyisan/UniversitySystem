using Core.Domain.Specifications;
using PersonMgmt.Domain.Aggregates;
using PersonMgmt.Domain.Enums;

namespace PersonMgmt.Domain.Specifications;

/// <summary>
/// Specification - Aktif öğrencileri getir
/// 
/// Kullanım:
/// var spec = new ActiveStudentsSpecification();
/// var persons = await _repository.GetAsync(spec);
/// </summary>
public class ActiveStudentsSpecification : Specification<Person>
{
    public ActiveStudentsSpecification()
    {
        Criteria = p => !p.IsDeleted &&
                        p.Student != null &&
                        p.Student.Status == StudentStatus.Active;

        AddOrderBy(p => p.Name);
    }
}