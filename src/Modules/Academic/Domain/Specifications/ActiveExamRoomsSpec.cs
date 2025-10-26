using Academic.Domain.Aggregates;
using Core.Domain.Specifications;

namespace Academic.Domain.Specifications;

/// <summary>
/// Specification for getting active exam rooms
/// </summary>
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