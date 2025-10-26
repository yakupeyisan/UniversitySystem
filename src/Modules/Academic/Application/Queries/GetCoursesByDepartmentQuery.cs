using Academic.Application.DTOs;
using Academic.Domain.Interfaces;
using AutoMapper;
using Core.Domain.Pagination;
using Core.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Academic.Application.Queries.Courses;

/// <summary>
/// Query to get all courses for a department
/// </summary>
public class GetCoursesByDepartmentQuery : IRequest<Result<PagedList<CourseListResponse>>>
{
    public Guid DepartmentId { get; set; }
    public PagedRequest PagedRequest { get; set; }

    public GetCoursesByDepartmentQuery(Guid departmentId, PagedRequest pagedRequest)
    {
        if (departmentId == Guid.Empty)
            throw new ArgumentException("Department ID cannot be empty", nameof(departmentId));

        DepartmentId = departmentId;
        PagedRequest = pagedRequest ?? throw new ArgumentNullException(nameof(pagedRequest));
    }

    public class Handler : IRequestHandler<GetCoursesByDepartmentQuery, Result<PagedList<CourseListResponse>>>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<Handler> _logger;

        public Handler(
            ICourseRepository courseRepository,
            IMapper mapper,
            ILogger<Handler> logger)
        {
            _courseRepository = courseRepository ?? throw new ArgumentNullException(nameof(courseRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<PagedList<CourseListResponse>>> Handle(
            GetCoursesByDepartmentQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                if (!request.PagedRequest.IsValid())
                {
                    _logger.LogWarning("Invalid pagination parameters");
                    return Result<PagedList<CourseListResponse>>.Failure("Invalid pagination parameters");
                }

                _logger.LogInformation(
                    "Fetching courses for department {DepartmentId} - Page: {PageNumber}, Size: {PageSize}",
                    request.DepartmentId,
                    request.PagedRequest.PageNumber,
                    request.PagedRequest.PageSize);

                var courses = await _courseRepository.GetByDepartmentAsync(
                    request.DepartmentId,
                    cancellationToken);

                var responses = _mapper.Map<List<CourseListResponse>>(courses);

                // Manual pagination (simple implementation)
                var totalCount = responses.Count;
                var pagedResults = responses
                    .Skip((request.PagedRequest.PageNumber - 1) * request.PagedRequest.PageSize)
                    .Take(request.PagedRequest.PageSize)
                    .ToList();

                var pagedList = new PagedList<CourseListResponse>(
                    pagedResults,
                    totalCount,
                    request.PagedRequest.PageNumber,
                    request.PagedRequest.PageSize);

                _logger.LogInformation(
                    "Retrieved {Count} courses for department {DepartmentId}",
                    courses.Count(),
                    request.DepartmentId);

                return Result<PagedList<CourseListResponse>>.Success(
                    pagedList,
                    "Courses retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching courses for department {DepartmentId}", request.DepartmentId);
                return Result<PagedList<CourseListResponse>>.Failure(ex.Message);
            }
        }
    }
}