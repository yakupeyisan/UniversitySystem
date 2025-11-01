using Core.Domain.Specifications;
using PersonMgmt.Domain.Aggregates;
using PersonMgmt.Domain.Enums;
namespace PersonMgmt.Domain.Specifications;
public class StaffByAcademicTitleSpecification : Specification<Person>
{
    public StaffByAcademicTitleSpecification(AcademicTitle academicTitle)
    {
        Criteria = p => !p.IsDeleted &&
                        p.Staff != null &&
                        p.Staff.IsActive &&
                        p.Staff.AcademicTitle == academicTitle;
        AddOrderBy(p => p.Name);
    }
}