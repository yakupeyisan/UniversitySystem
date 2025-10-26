using Core.Domain;
using Academic.Domain.Aggregates;
using Core.Domain.Specifications;

namespace Academic.Domain.Specifications;

/// <summary>
/// Specification for getting course by course code
/// </summary>
public class CourseByCodeSpec : Specification<Course>
{
    public CourseByCodeSpec(string courseCode)
    {
        Criteria = c => c.Code.Value == courseCode && !c.IsDeleted;
    }
}

