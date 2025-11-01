using Academic.Domain.Aggregates;
using Core.Domain.Specifications;
namespace Academic.Domain.Specifications;
public class ExamRoomByRoomNumberSpec : Specification<ExamRoom>
{
    public ExamRoomByRoomNumberSpec(string roomNumber)
    {
        Criteria = er => er.RoomNumber == roomNumber;
    }
}