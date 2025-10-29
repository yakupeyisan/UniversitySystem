using Academic.Domain.Aggregates;
using Core.Domain.Specifications;

namespace Academic.Domain.Specifications;

public class ExamRoomsByCapacitySpec : Specification<ExamRoom>
{
    public ExamRoomsByCapacitySpec(int minCapacity)
    {
        Criteria = er => er.Capacity >= minCapacity && er.IsActive;
        AddOrderByDescending(er => er.Capacity);
    }
}