using Academic.Application.DTOs;
using Academic.Domain.Interfaces;
using AutoMapper;
using Core.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;
namespace Academic.Application.Queries.Courses;
public class GetStudentWaitingListPositionsQuery : IRequest<Result<IEnumerable<CourseWaitingListEntryResponse>>>
{
    public Guid StudentId { get; set; }
    public GetStudentWaitingListPositionsQuery(Guid studentId)
    {
        if (studentId == Guid.Empty)
            throw new ArgumentException("Student ID cannot be empty", nameof(studentId));
        StudentId = studentId;
    }
    public class Handler : IRequestHandler<GetStudentWaitingListPositionsQuery, Result<IEnumerable<CourseWaitingListEntryResponse>>>
    {
        private readonly IWaitingListRepository _waitingListRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<Handler> _logger;
        public Handler(IWaitingListRepository waitingListRepository, IMapper mapper, ILogger<Handler> logger)
        {
            _waitingListRepository = waitingListRepository ?? throw new ArgumentNullException(nameof(waitingListRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task<Result<IEnumerable<CourseWaitingListEntryResponse>>> Handle(
            GetStudentWaitingListPositionsQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching waiting list positions for student: {StudentId}",
                    request.StudentId);
                var entries = await _waitingListRepository.GetByStudentAsync(
                    request.StudentId, cancellationToken);
                var responses = _mapper.Map<IEnumerable<CourseWaitingListEntryResponse>>(entries);
                _logger.LogInformation("Retrieved {Count} waiting list positions for student",
                    entries.Count());
                return Result<IEnumerable<CourseWaitingListEntryResponse>>.Success(responses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching waiting list positions for student: {StudentId}",
                    request.StudentId);
                return Result<IEnumerable<CourseWaitingListEntryResponse>>.Failure(ex.Message);
            }
        }
    }
}