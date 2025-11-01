using Academic.Application.DTOs;
using Academic.Domain.Aggregates;
using Academic.Domain.Specifications;
using Core.Domain.Repositories;
using Core.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;
namespace Academic.Application.Commands.Courses;
public class DropCourseCommand : IRequest<Result<Unit>>
{
    public DropCourseCommand(DropCourseRequest request)
    {
        Request = request ?? throw new ArgumentNullException(nameof(request));
    }
    public DropCourseRequest Request { get; set; }
    public class Handler : IRequestHandler<DropCourseCommand, Result<Unit>>
    {
        private readonly ILogger<Handler> _logger;
        private readonly IRepository<CourseRegistration> _registrationRepository;
        public Handler(
            IRepository<CourseRegistration> registrationRepository,
            ILogger<Handler> logger)
        {
            _registrationRepository =
                registrationRepository ?? throw new ArgumentNullException(nameof(registrationRepository));
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
                var registration = await _registrationRepository.GetAsync(
                    new CourseRegistrationByStudentAndCourseSpec(request.Request.StudentId, request.Request.CourseId),
                    cancellationToken);
                if (registration == null)
                {
                    _logger.LogWarning(
                        "Registration not found for student {StudentId} and course {CourseId}",
                        request.Request.StudentId,
                        request.Request.CourseId);
                    return Result<Unit>.Failure("Student is not registered for this course");
                }
                registration.Drop(request.Request.Reason);
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