using Academic.Application.DTOs;
using Academic.Domain.Interfaces;
using AutoMapper;
using Core.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Academic.Application.Queries.Courses;

public class GetStudentGradesBySemesterQuery : IRequest<Result<IEnumerable<GradeResponse>>>
{
    public Guid StudentId { get; set; }
    public string Semester { get; set; }

    public GetStudentGradesBySemesterQuery(Guid studentId, string semester)
    {
        if (studentId == Guid.Empty)
            throw new ArgumentException("Student ID cannot be empty", nameof(studentId));
        if (string.IsNullOrWhiteSpace(semester))
            throw new ArgumentException("Semester cannot be empty", nameof(semester));

        StudentId = studentId;
        Semester = semester;
    }

    public class Handler : IRequestHandler<GetStudentGradesBySemesterQuery, Result<IEnumerable<GradeResponse>>>
    {
        private readonly IGradeRepository _gradeRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<Handler> _logger;

        public Handler(IGradeRepository gradeRepository, IMapper mapper, ILogger<Handler> logger)
        {
            _gradeRepository = gradeRepository ?? throw new ArgumentNullException(nameof(gradeRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<IEnumerable<GradeResponse>>> Handle(
            GetStudentGradesBySemesterQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching grades for student {StudentId} in semester {Semester}",
                    request.StudentId, request.Semester);

                var grades = await _gradeRepository.GetByStudentAndSemesterAsync(
                    request.StudentId, request.Semester, cancellationToken);

                var responses = _mapper.Map<IEnumerable<GradeResponse>>(grades);

                _logger.LogInformation("Retrieved {Count} grades for student in semester",
                    grades.Count());

                return Result<IEnumerable<GradeResponse>>.Success(responses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching grades for student in semester");
                return Result<IEnumerable<GradeResponse>>.Failure(ex.Message);
            }
        }
    }
}