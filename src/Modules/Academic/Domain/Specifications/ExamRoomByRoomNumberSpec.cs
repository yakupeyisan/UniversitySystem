using Academic.Domain.Aggregates;
using Core.Domain.Specifications;

namespace Academic.Domain.Specifications;

/// <summary>
/// Specification for getting exam room by room number
/// </summary>
public class ExamRoomByRoomNumberSpec : Specification<ExamRoom>
{
    public ExamRoomByRoomNumberSpec(string roomNumber)
    {
        Criteria = er => er.RoomNumber == roomNumber;
    }
}