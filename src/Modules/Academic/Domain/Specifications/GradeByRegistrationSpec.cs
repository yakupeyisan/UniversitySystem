using Academic.Domain.Aggregates;
using Core.Domain.Specifications;

namespace Academic.Domain.Specifications;

/// <summary>
/// Specification for getting grades by registration
/// </summary>
public class GradeByRegistrationSpec : Specification<Grade>
{
    public GradeByRegistrationSpec(Guid registrationId)
    {
        Criteria = g => g.RegistrationId == registrationId && !g.IsDeleted;
    }
}