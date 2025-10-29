using Academic.Application.DTOs;
using Academic.Domain.Aggregates;
using Academic.Domain.Specifications;
using AutoMapper;
using Core.Domain.Repositories;
using Core.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Academic.Application.Queries.Courses;

public class GetCourseWaitingListQuery : IRequest<Result<IEnumerable<CourseWaitingListEntryResponse>>>
{
    public GetCourseWaitingListQuery(Guid courseId)
    {
        if (courseId == Guid.Empty)
            throw new ArgumentException("Course ID cannot be empty", nameof(courseId));
        CourseId = courseId;
    }

    public Guid CourseId { get; set; }

    public class Handler : IRequestHandler<GetCourseWaitingListQuery,
        Result<IEnumerable<CourseWaitingListEntryResponse>>>
    {
        private readonly ILogger<Handler> _logger;
        private readonly IMapper _mapper;

        private readonly IRepository<CourseWaitingListEntry>
            _waitingListRepository;

        public Handler(IRepository<CourseWaitingListEntry>
            waitingListRepository, IMapper mapper, ILogger<Handler> logger)
        {
            _waitingListRepository =
                waitingListRepository ?? throw new ArgumentNullException(nameof(waitingListRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<IEnumerable<CourseWaitingListEntryResponse>>> Handle(
            GetCourseWaitingListQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching waiting list for course: {CourseId}", request.CourseId);
                var entries = await _waitingListRepository.GetAllAsync(
                    new WaitingListByCourseSpec(request.CourseId), cancellationToken);
                var responses = _mapper.Map<IEnumerable<CourseWaitingListEntryResponse>>(entries);
                _logger.LogInformation("Retrieved {Count} entries in waiting list for course",
                    entries.Count());
                return Result<IEnumerable<CourseWaitingListEntryResponse>>.Success(responses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching waiting list for course: {CourseId}",
                    request.CourseId);
                return Result<IEnumerable<CourseWaitingListEntryResponse>>.Failure(ex.Message);
            }
        }
    }
}