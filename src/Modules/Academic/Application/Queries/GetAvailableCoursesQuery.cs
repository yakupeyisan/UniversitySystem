using Academic.Application.DTOs;
using Academic.Domain.Interfaces;
using AutoMapper;
using Core.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;
namespace Academic.Application.Queries.Courses;
public class GetAvailableCoursesQuery : IRequest<Result<IEnumerable<CourseListResponse>>>
{
    public GetAvailableCoursesQuery() { }
    public class Handler : IRequestHandler<GetAvailableCoursesQuery, Result<IEnumerable<CourseListResponse>>>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<Handler> _logger;
        public Handler(ICourseRepository courseRepository, IMapper mapper, ILogger<Handler> logger)
        {
            _courseRepository = courseRepository ?? throw new ArgumentNullException(nameof(courseRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task<Result<IEnumerable<CourseListResponse>>> Handle(
            GetAvailableCoursesQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching available courses");
                var courses = await _courseRepository.GetAvailableCoursesAsync(cancellationToken);
                var responses = _mapper.Map<IEnumerable<CourseListResponse>>(courses);
                _logger.LogInformation("Retrieved {Count} available courses", courses.Count());
                return Result<IEnumerable<CourseListResponse>>.Success(responses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching available courses");
                return Result<IEnumerable<CourseListResponse>>.Failure(ex.Message);
            }
        }
    }
}