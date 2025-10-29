using Academic.Application.DTOs;
using Academic.Domain.Aggregates;
using Academic.Domain.Specifications;
using AutoMapper;
using Core.Domain.Repositories;
using Core.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Academic.Application.Commands.Courses;

public class JoinWaitingListCommand : IRequest<Result<WaitingListResponse>>
{
    public JoinWaitingListCommand(JoinWaitingListRequest request)
    {
        Request = request ?? throw new ArgumentNullException(nameof(request));
    }

    public JoinWaitingListRequest Request { get; set; }

    public class Handler : IRequestHandler<JoinWaitingListCommand, Result<WaitingListResponse>>
    {
        private readonly IRepository<Course> _courseRepository;
        private readonly ILogger<Handler> _logger;
        private readonly IMapper _mapper;
        private readonly IRepository<CourseRegistration> _registrationRepository;
        private readonly IRepository<CourseWaitingListEntry> _waitingListRepository;

        public Handler(
            IRepository<CourseWaitingListEntry> waitingListRepository,
            IRepository<CourseRegistration> registrationRepository,
            IRepository<Course> courseRepository,
            IMapper mapper,
            ILogger<Handler> logger)
        {
            _registrationRepository =
                registrationRepository ?? throw new ArgumentNullException(nameof(registrationRepository));
            _courseRepository = courseRepository ?? throw new ArgumentNullException(nameof(courseRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _waitingListRepository = waitingListRepository;
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

                var existingEntry = await _waitingListRepository.GetAsync(
                    new WaitingListByStudentAndCourseSpec(request.Request.StudentId,
                        request.Request.CourseId),
                    cancellationToken);
                if (existingEntry != null)
                {
                    _logger.LogWarning(
                        "Student {StudentId} is already on waiting list for course {CourseId}",
                        request.Request.StudentId,
                        request.Request.CourseId);
                    return Result<WaitingListResponse>.Failure(
                        "Student is already on the waiting list for this course");
                }

                var spec = new WaitingListByCourseSpec(request.Request.CourseId);
                var result = await _waitingListRepository.GetAllAsync(spec, cancellationToken);
                var entries = result.ToList();
                var nextQueuePosition = entries.Any() ? entries.Max(e => e.QueuePosition) + 1 : 1;
                _logger.LogInformation(
                    "Next queue position for course {CourseId} is {Position}",
                    request.Request.CourseId,
                    nextQueuePosition);
                var waitingListEntry = CourseWaitingListEntry.Create(
                    request.Request.StudentId,
                    request.Request.CourseId,
                    nextQueuePosition);
                await _waitingListRepository.AddAsync(waitingListEntry, cancellationToken);
                await _waitingListRepository.SaveChangesAsync(cancellationToken);
                _logger.LogInformation(
                    "Student {StudentId} added to waiting list for course {CourseId} at position {Position}",
                    request.Request.StudentId,
                    request.Request.CourseId,
                    nextQueuePosition);
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