using Academic.Application.DTOs;
using Academic.Domain.Aggregates;
using Academic.Domain.Interfaces;
using AutoMapper;
using Core.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Academic.Application.Queries.Courses;

/// <summary>
/// Query to get all exams for a student
/// </summary>
public class GetExamsByStudentQuery : IRequest<Result<List<ExamResponse>>>
{
    public Guid StudentId { get; set; }

    public GetExamsByStudentQuery(Guid studentId)
    {
        if (studentId == Guid.Empty)
            throw new ArgumentException("Student ID cannot be empty", nameof(studentId));
        StudentId = studentId;
    }

    public class Handler : IRequestHandler<GetExamsByStudentQuery, Result<List<ExamResponse>>>
    {
        private readonly IExamRepository _examRepository;
        private readonly ICourseRegistrationRepository _registrationRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<Handler> _logger;

        public Handler(
            IExamRepository examRepository,
            ICourseRegistrationRepository registrationRepository,
            IMapper mapper,
            ILogger<Handler> logger)
        {
            _examRepository = examRepository ?? throw new ArgumentNullException(nameof(examRepository));
            _registrationRepository = registrationRepository ?? throw new ArgumentNullException(nameof(registrationRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<List<ExamResponse>>> Handle(
            GetExamsByStudentQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching exams for student {StudentId}", request.StudentId);

                // Get student's registered courses
                var registrations = await _registrationRepository.GetByStudentAsync(
                    request.StudentId,
                    cancellationToken);

                var courseIds = registrations.Select(r => r.CourseId).ToList();

                if (courseIds.Count == 0)
                {
                    _logger.LogInformation("Student {StudentId} has no registered courses", request.StudentId);
                    return Result<List<ExamResponse>>.Success(
                        new List<ExamResponse>(),
                        "Student has no registered courses");
                }

                // Get exams for those courses
                var exams = new List<Exam>();
                foreach (var courseId in courseIds)
                {
                    var courseExams = await _examRepository.GetByCourseAsync(courseId, cancellationToken);
                    exams.AddRange(courseExams);
                }

                var responses = _mapper.Map<List<ExamResponse>>(exams);

                _logger.LogInformation(
                    "Retrieved {Count} exams for student {StudentId}",
                    responses.Count,
                    request.StudentId);

                return Result<List<ExamResponse>>.Success(
                    responses,
                    "Exams retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching exams for student {StudentId}", request.StudentId);
                return Result<List<ExamResponse>>.Failure(ex.Message);
            }
        }
    }
}