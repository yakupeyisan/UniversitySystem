using Academic.Domain.Aggregates;
using Core.Domain.Specifications;

namespace Academic.Domain.Specifications;

/// <summary>
/// Specification for getting exam rooms by minimum capacity
/// </summary>
public class ExamRoomsByCapacitySpec : Specification<ExamRoom>
{
    public ExamRoomsByCapacitySpec(int minCapacity)
    {
        Criteria = er => er.Capacity >= minCapacity && er.IsActive;
        AddOrderByDescending(er => er.Capacity);
    }
}