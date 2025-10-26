using Academic.Application.DTOs;
using Academic.Domain.Interfaces;
using Core.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Academic.Application.Commands.Courses;

/// <summary>
/// Command to drop a course
/// </summary>
public class DropCourseCommand : IRequest<Result<Unit>>
{
    public DropCourseRequest Request { get; set; }

    public DropCourseCommand(DropCourseRequest request)
    {
        Request = request ?? throw new ArgumentNullException(nameof(request));
    }

    public class Handler : IRequestHandler<DropCourseCommand, Result<Unit>>
    {
        private readonly ICourseRegistrationRepository _registrationRepository;
        private readonly ILogger<Handler> _logger;

        public Handler(
            ICourseRegistrationRepository registrationRepository,
            ILogger<Handler> logger)
        {
            _registrationRepository = registrationRepository ?? throw new ArgumentNullException(nameof(registrationRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<Unit>> Handle(
            DropCourseCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation(
                    "Dropping course {CourseId} for student {StudentId}",
                    request.Request.CourseId,
                    request.Request.StudentId);

                var registration = await _registrationRepository.GetByStudentAndCourseAsync(
                    request.Request.StudentId,
                    request.Request.CourseId,
                    cancellationToken);

                if (registration == null)
                {
                    _logger.LogWarning(
                        "Registration not found for student {StudentId} and course {CourseId}",
                        request.Request.StudentId,
                        request.Request.CourseId);
                    return Result<Unit>.Failure("Student is not registered for this course");
                }

                // Drop course
                registration.Drop(request.Request.Reason);

                // Save changes
                await _registrationRepository.UpdateAsync(registration, cancellationToken);
                await _registrationRepository.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Course {CourseId} dropped successfully for student {StudentId}",
                    request.Request.CourseId,
                    request.Request.StudentId);

                return Result<Unit>.Success(Unit.Value, "Course dropped successfully");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Business logic error while dropping course");
                return Result<Unit>.Failure(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error dropping course");
                return Result<Unit>.Failure(ex.Message);
            }
        }
    }
}