using Academic.Application.DTOs;
using AutoMapper;
using Core.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Academic.Application.Queries.Courses;

public class GetCourseQuery : IRequest<Result<CourseResponse>>
{
    public GetCourseQuery(Guid courseId)
    {
        if (courseId == Guid.Empty)
            throw new ArgumentException("Course ID cannot be empty", nameof(courseId));
        CourseId = courseId;
    }

    public Guid CourseId { get; set; }

    public class Handler : IRequestHandler<GetCourseQuery, Result<CourseResponse>>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly ILogger<Handler> _logger;
        private readonly IMapper _mapper;

        public Handler(
            ICourseRepository courseRepository,
            IMapper mapper,
            ILogger<Handler> logger)
        {
            _courseRepository = courseRepository ?? throw new ArgumentNullException(nameof(courseRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<CourseResponse>> Handle(
            GetCourseQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching course with ID: {CourseId}", request.CourseId);
                var course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);
                if (course == null)
                {
                    _logger.LogWarning("Course not found with ID: {CourseId}", request.CourseId);
                    return Result<CourseResponse>.Failure(
                        $"Course with ID {request.CourseId} not found");
                }

                var response = _mapper.Map<CourseResponse>(course);
                _logger.LogInformation("Course retrieved successfully with ID: {CourseId}", course.Id);
                return Result<CourseResponse>.Success(response, "Course retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching course with ID: {CourseId}", request.CourseId);
                return Result<CourseResponse>.Failure(ex.Message);
            }
        }
    }
}