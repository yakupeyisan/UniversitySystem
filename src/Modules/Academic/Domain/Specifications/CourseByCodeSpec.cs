using Core.Domain;
using Academic.Domain.Aggregates;
using Core.Domain.Specifications;

namespace Academic.Domain.Specifications;

#region Course Specifications

/// <summary>
/// Specification for getting course by course code
/// </summary>
public class CourseByCodeSpec : Specification<Course>
{
    public CourseByCodeSpec(string courseCode)
    {
        Criteria = c => c.CourseCode == courseCode && !c.IsDeleted;
    }
}

#endregion

#region CourseRegistration Specifications

#endregion

#region WaitingList Specifications

#endregion

#region Exam Specifications

#endregion

#region Grade Specifications

#endregion

#region GradeObjection Specifications

#endregion

#region PrerequisiteWaiver Specifications

#endregion

#region Prerequisite Specifications

#endregion

#region ExamRoom Specifications

#endregion


