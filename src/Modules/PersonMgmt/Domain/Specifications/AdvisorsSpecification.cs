using Core.Domain.Specifications;
using PersonMgmt.Domain.Aggregates;
using PersonMgmt.Domain.Enums;

namespace PersonMgmt.Domain.Specifications;

/// <summary>
/// Specification - Danışman olarak görev yapan personeli getir
/// </summary>
public class AdvisorsSpecification : Specification<Person>
{
    public AdvisorsSpecification()
    {
        Criteria = p => !p.IsDeleted &&
                        p.Staff != null &&
                        p.Staff.IsActive &&
                        p.Staff.AcademicTitle != AcademicTitle.ResearchAssistant;

        AddOrderBy(p => p.Name);
    }
}