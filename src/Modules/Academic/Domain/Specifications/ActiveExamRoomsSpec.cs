using Academic.Domain.Aggregates;
using Core.Domain.Specifications;

namespace Academic.Domain.Specifications;

public class ActiveExamRoomsSpec : Specification<ExamRoom>
{
    public ActiveExamRoomsSpec()
    {
        Criteria = er => er.IsActive;
        AddOrderBy(er => er.Building);
        AddOrderBy(er => er.Floor);
        AddOrderBy(er => er.RoomNumber);
    }
}