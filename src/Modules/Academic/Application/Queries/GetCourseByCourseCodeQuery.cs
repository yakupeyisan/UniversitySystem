using Academic.Application.DTOs;
using Academic.Domain.Interfaces;
using AutoMapper;
using Core.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Academic.Application.Queries.Courses;

public class GetCourseByCourseCodeQuery : IRequest<Result<CourseResponse>>
{
    public string CourseCode { get; set; }

    public GetCourseByCourseCodeQuery(string courseCode)
    {
        if (string.IsNullOrWhiteSpace(courseCode))
            throw new ArgumentException("Course code cannot be empty", nameof(courseCode));
        CourseCode = courseCode;
    }

    public class Handler : IRequestHandler<GetCourseByCourseCodeQuery, Result<CourseResponse>>
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

        public async Task<Result<CourseResponse>> Handle(
            GetCourseByCourseCodeQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching course with code: {CourseCode}", request.CourseCode);

                var course = await _courseRepository.GetByCourseCodeAsync(request.CourseCode, cancellationToken);

                if (course == null)
                {
                    _logger.LogWarning("Course not found with code: {CourseCode}", request.CourseCode);
                    return Result<CourseResponse>.Failure(
                        $"Course with code {request.CourseCode} not found");
                }

                var response = _mapper.Map<CourseResponse>(course);
                return Result<CourseResponse>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching course with code: {CourseCode}", request.CourseCode);
                return Result<CourseResponse>.Failure(ex.Message);
            }
        }
    }
}