using Academic.Application.DTOs;
using Academic.Domain.Aggregates;
using Academic.Domain.Specifications;
using AutoMapper;
using Core.Domain.Repositories;
using Core.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Academic.Application.Queries.Courses;

public class GetStudentGradeForCourseQuery : IRequest<Result<GradeResponse>>
{
    public GetStudentGradeForCourseQuery(Guid studentId, Guid courseId)
    {
        if (studentId == Guid.Empty)
            throw new ArgumentException("Student ID cannot be empty", nameof(studentId));
        if (courseId == Guid.Empty)
            throw new ArgumentException("Course ID cannot be empty", nameof(courseId));
        StudentId = studentId;
        CourseId = courseId;
    }

    public Guid StudentId { get; set; }
    public Guid CourseId { get; set; }

    public class Handler : IRequestHandler<GetStudentGradeForCourseQuery, Result<GradeResponse>>
    {
        private readonly IRepository<Grade>
            _gradeRepository;

        private readonly ILogger<Handler> _logger;
        private readonly IMapper _mapper;

        public Handler(IRepository<Grade>
            gradeRepository, IMapper mapper, ILogger<Handler> logger)
        {
            _gradeRepository = gradeRepository ?? throw new ArgumentNullException(nameof(gradeRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<GradeResponse>> Handle(
            GetStudentGradeForCourseQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching grade for student {StudentId} in course {CourseId}",
                    request.StudentId, request.CourseId);
                var grade = await _gradeRepository.GetAsync(
                    new GradeByStudentAndCourseSpec(request.StudentId, request.CourseId), cancellationToken);
                if (grade == null)
                {
                    _logger.LogWarning("Grade not found for student {StudentId} in course {CourseId}",
                        request.StudentId, request.CourseId);
                    return Result<GradeResponse>.Failure("Grade not found");
                }

                var response = _mapper.Map<GradeResponse>(grade);
                return Result<GradeResponse>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching grade for student in course");
                return Result<GradeResponse>.Failure(ex.Message);
            }
        }
    }
}