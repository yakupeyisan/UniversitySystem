using Academic.Domain.Aggregates;
using Core.Domain.Specifications;
namespace Academic.Domain.Specifications;
public class ExamRoomsByBuildingSpec : Specification<ExamRoom>
{
    public ExamRoomsByBuildingSpec(string building)
    {
        Criteria = er => er.Building == building && er.IsActive;
        AddOrderBy(er => er.Floor);
        AddOrderBy(er => er.RoomNumber);
    }
}