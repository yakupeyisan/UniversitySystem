using Academic.Application.DTOs;
using Academic.Domain.Aggregates;
using Academic.Domain.Specifications;
using AutoMapper;
using Core.Domain.Repositories;
using Core.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;
namespace Academic.Application.Queries.Courses;
public class GetExamsByCourseQuery : IRequest<Result<IEnumerable<ExamResponse>>>
{
    public GetExamsByCourseQuery(Guid courseId)
    {
        if (courseId == Guid.Empty)
            throw new ArgumentException("Course ID cannot be empty", nameof(courseId));
        CourseId = courseId;
    }
    public Guid CourseId { get; set; }
    public class Handler : IRequestHandler<GetExamsByCourseQuery, Result<IEnumerable<ExamResponse>>>
    {
        private readonly IRepository<Exam>
            _examRepository;
        private readonly ILogger<Handler> _logger;
        private readonly IMapper _mapper;
        public Handler(IRepository<Exam>
            examRepository, IMapper mapper, ILogger<Handler> logger)
        {
            _examRepository = examRepository ?? throw new ArgumentNullException(nameof(examRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task<Result<IEnumerable<ExamResponse>>> Handle(
            GetExamsByCourseQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching exams for course: {CourseId}", request.CourseId);
                var exams = await _examRepository.GetAllAsync(new ExamByCourseSpec(request.CourseId),
                    cancellationToken);
                var responses = _mapper.Map<IEnumerable<ExamResponse>>(exams);
                _logger.LogInformation("Retrieved {Count} exams for course: {CourseId}",
                    exams.Count(), request.CourseId);
                return Result<IEnumerable<ExamResponse>>.Success(responses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching exams for course: {CourseId}", request.CourseId);
                return Result<IEnumerable<ExamResponse>>.Failure(ex.Message);
            }
        }
    }
}