using Core.Domain.Exceptions;

namespace Academic.Domain.Exceptions;

/// <summary>
/// Exception thrown when a course registration is not found
/// </summary>
public class CourseRegistrationNotFoundException : DomainException
{
    public Guid RegistrationId { get; }

    public CourseRegistrationNotFoundException(Guid registrationId)
        : base($"Course registration with ID {registrationId} was not found.")
    {
        RegistrationId = registrationId;
    }

    public override string ErrorCode => "errors.course.registration.not.found";
    public override int StatusCode => 404;
}