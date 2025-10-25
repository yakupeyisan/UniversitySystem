using Core.Domain.Specifications;
using PersonMgmt.Domain.Aggregates;
using PersonMgmt.Domain.Enums;

namespace PersonMgmt.Domain.Specifications;

public class StudentsByGpaRangeSpecification : Specification<Person>
{
    public StudentsByGpaRangeSpecification(double minGpa, double maxGpa)
    {
        Criteria = p => !p.IsDeleted &&
                        p.Student != null &&
                        p.Student.CGPA >= minGpa &&
                        p.Student.CGPA <= maxGpa &&
                        p.Student.Status == StudentStatus.Active;

        AddOrderByDescending(p => p.Student.CGPA);
    }
}