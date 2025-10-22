using Core.Domain.Specifications;
using PersonMgmt.Domain.Aggregates;
using PersonMgmt.Domain.Enums;

namespace PersonMgmt.Domain.Specifications;

/// <summary>
/// Specification - Öğrencileri eğitim seviyesine göre getir
/// </summary>
public class StudentsByEducationLevelSpecification : Specification<Person>
{
    public StudentsByEducationLevelSpecification(EducationLevel educationLevel)
    {
        Criteria = p => !p.IsDeleted &&
                        p.Student != null &&
                        p.Student.EducationLevel == educationLevel;

        AddOrderBy(p => p.Name);
    }
}