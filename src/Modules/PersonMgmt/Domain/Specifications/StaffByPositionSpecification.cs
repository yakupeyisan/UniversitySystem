using Core.Domain.Specifications;
using PersonMgmt.Domain.Aggregates;
using PersonMgmt.Domain.Enums;
namespace PersonMgmt.Domain.Specifications;
public class StaffByPositionSpecification : Specification<Person>
{
    public StaffByPositionSpecification(string position)
    {
        if (Enum.TryParse<AcademicTitle>(position, true, out var academicTitle))
            Criteria = p => !p.IsDeleted &&
                            p.Staff != null &&
                            p.Staff.IsActive &&
                            p.Staff.AcademicTitle == academicTitle;
        else
            Criteria = p => false;
        AddOrderBy(p => p.Name.FirstName);
    }
}