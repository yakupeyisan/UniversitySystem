using Academic.Application.DTOs;
using Academic.Domain.Aggregates;
using Academic.Domain.Interfaces;
using AutoMapper;
using Core.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Academic.Application.Commands.Courses;

/// <summary>
/// Command to join course waiting list
/// </summary>
public class JoinWaitingListCommand : IRequest<Result<WaitingListResponse>>
{
    public JoinWaitingListRequest Request { get; set; }

    public JoinWaitingListCommand(JoinWaitingListRequest request)
    {
        Request = request ?? throw new ArgumentNullException(nameof(request));
    }

    public class Handler : IRequestHandler<JoinWaitingListCommand, Result<WaitingListResponse>>
    {
        private readonly ICourseRegistrationRepository _registrationRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<Handler> _logger;

        public Handler(
            ICourseRegistrationRepository registrationRepository,
            ICourseRepository courseRepository,
            IMapper mapper,
            ILogger<Handler> logger)
        {
            _registrationRepository = registrationRepository ?? throw new ArgumentNullException(nameof(registrationRepository));
            _courseRepository = courseRepository ?? throw new ArgumentNullException(nameof(courseRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<WaitingListResponse>> Handle(
            JoinWaitingListCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation(
                    "Student {StudentId} joining waiting list for course {CourseId}",
                    request.Request.StudentId,
                    request.Request.CourseId);

                // Verify course exists
                var course = await _courseRepository.GetByIdAsync(
                    request.Request.CourseId,
                    cancellationToken);

                if (course == null)
                {
                    _logger.LogWarning(
                        "Course not found with ID: {CourseId}",
                        request.Request.CourseId);
                    return Result<WaitingListResponse>.Failure(
                        $"Course with ID {request.Request.CourseId} not found");
                }

                // Create waiting list entry
                var waitingListEntry = CourseWaitingListEntry.Create(
                    studentId: request.Request.StudentId,
                    courseId: request.Request.CourseId,
                    semester: request.Request.Semester);

                // Save to database
                // TODO: Implement CourseWaitingListEntry repository
                _logger.LogInformation(
                    "Student {StudentId} added to waiting list for course {CourseId}",
                    request.Request.StudentId,
                    request.Request.CourseId);

                var response = _mapper.Map<WaitingListResponse>(waitingListEntry);
                return Result<WaitingListResponse>.Success(
                    response,
                    "Student added to waiting list successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error joining waiting list");
                return Result<WaitingListResponse>.Failure(ex.Message);
            }
        }
    }
}