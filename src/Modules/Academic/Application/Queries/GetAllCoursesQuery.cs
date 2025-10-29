using Academic.Application.DTOs;
using Academic.Domain.Aggregates;
using AutoMapper;
using Core.Domain.Pagination;
using Core.Domain.Repositories;
using Core.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Academic.Application.Queries.Courses;

public class GetAllCoursesQuery : IRequest<Result<PagedList<CourseListResponse>>>
{
    public GetAllCoursesQuery(PagedRequest pagedRequest)
    {
        PagedRequest = pagedRequest ?? throw new ArgumentNullException(nameof(pagedRequest));
    }

    public PagedRequest PagedRequest { get; set; }

    public class Handler : IRequestHandler<GetAllCoursesQuery, Result<PagedList<CourseListResponse>>>
    {
        private readonly IRepository<Course>
            _courseRepository;

        private readonly ILogger<Handler> _logger;
        private readonly IMapper _mapper;

        public Handler(IRepository<Course>
            courseRepository, IMapper mapper, ILogger<Handler> logger)
        {
            _courseRepository = courseRepository ?? throw new ArgumentNullException(nameof(courseRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<PagedList<CourseListResponse>>> Handle(
            GetAllCoursesQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching all courses - Page: {PageNumber}, Size: {PageSize}",
                    request.PagedRequest.PageNumber, request.PagedRequest.PageSize);
                var result = await _courseRepository.GetAllAsync(
                    request.PagedRequest,
                    cancellationToken);
                var responses = _mapper.Map<List<CourseListResponse>>(result.Data);
                var pagedResult = new PagedList<CourseListResponse>(responses, result.TotalCount,
                    request.PagedRequest.PageNumber, request.PagedRequest.PageSize);
                _logger.LogInformation("Retrieved {Count} courses (Total: {Total})",
                    responses.Count, result.TotalCount);
                return Result<PagedList<CourseListResponse>>.Success(pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all courses");
                return Result<PagedList<CourseListResponse>>.Failure(ex.Message);
            }
        }
    }
}