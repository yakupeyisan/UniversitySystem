using Academic.Application.DTOs;
using Academic.Domain.Interfaces;
using AutoMapper;
using Core.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;
namespace Academic.Application.Queries.Courses;
public class GetStudentGradesQuery : IRequest<Result<StudentGradesResponse>>
{
    public Guid StudentId { get; set; }
    public GetStudentGradesQuery(Guid studentId)
    {
        if (studentId == Guid.Empty)
            throw new ArgumentException("Student ID cannot be empty", nameof(studentId));
        StudentId = studentId;
    }
    public class Handler : IRequestHandler<GetStudentGradesQuery, Result<StudentGradesResponse>>
    {
        private readonly IGradeRepository _gradeRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<Handler> _logger;
        public Handler(
            IGradeRepository gradeRepository,
            IMapper mapper,
            ILogger<Handler> logger)
        {
            _gradeRepository = gradeRepository ?? throw new ArgumentNullException(nameof(gradeRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task<Result<StudentGradesResponse>> Handle(
            GetStudentGradesQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching grades for student {StudentId}", request.StudentId);
                var grades = await _gradeRepository.GetByStudentAsync(
                    request.StudentId,
                    cancellationToken);
                var gradeResponses = _mapper.Map<List<GradeResponse>>(grades);
                var totalEcts = grades.Sum(g => g.ECTS);
                var cumulativeGpa = grades.Any()
                    ? grades.Average(g => g.GradePoint)
                    : 0f;
                var response = new StudentGradesResponse
                {
                    StudentId = request.StudentId,
                    Grades = gradeResponses,
                    CumulativeGPA = (float)cumulativeGpa,
                    TotalECTS = totalEcts
                };
                _logger.LogInformation(
                    "Retrieved {Count} grades for student {StudentId}. GPA: {GPA}",
                    grades.Count(),
                    request.StudentId,
                    cumulativeGpa);
                return Result<StudentGradesResponse>.Success(
                    response,
                    "Student grades retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching grades for student {StudentId}", request.StudentId);
                return Result<StudentGradesResponse>.Failure(ex.Message);
            }
        }
    }
}