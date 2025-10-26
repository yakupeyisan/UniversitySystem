using Academic.Application.DTOs;
using Academic.Domain.Interfaces;
using AutoMapper;
using Core.Domain.Pagination;
using Core.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Academic.Application.Queries.Courses;

/// <summary>
/// Query to get all courses for a student
/// </summary>
public class GetStudentCoursesQuery : IRequest<Result<StudentCoursesResponse>>
{
    public Guid StudentId { get; set; }
    public PagedRequest? PagedRequest { get; set; }

    public GetStudentCoursesQuery(Guid studentId, PagedRequest? pagedRequest = null)
    {
        if (studentId == Guid.Empty)
            throw new ArgumentException("Student ID cannot be empty", nameof(studentId));
        StudentId = studentId;
        PagedRequest = pagedRequest;
    }

    public class Handler : IRequestHandler<GetStudentCoursesQuery, Result<StudentCoursesResponse>>
    {
        private readonly ICourseRegistrationRepository _registrationRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<Handler> _logger;

        public Handler(
            ICourseRegistrationRepository registrationRepository,
            IMapper mapper,
            ILogger<Handler> logger)
        {
            _registrationRepository = registrationRepository ?? throw new ArgumentNullException(nameof(registrationRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<StudentCoursesResponse>> Handle(
            GetStudentCoursesQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching courses for student {StudentId}", request.StudentId);

                var registrations = await _registrationRepository.GetByStudentAsync(
                    request.StudentId,
                    cancellationToken);

                var courseResponses = _mapper.Map<List<CourseRegistrationResponse>>(registrations);
                var totalEcts = registrations.Sum(r => r.Course?.Credits ?? 0);

                var response = new StudentCoursesResponse
                {
                    StudentId = request.StudentId,
                    Courses = courseResponses,
                    TotalEnrolledCourses = registrations.Count(),
                    TotalECTS = totalEcts
                };

                _logger.LogInformation(
                    "Retrieved {Count} courses for student {StudentId}",
                    registrations.Count(),
                    request.StudentId);

                return Result<StudentCoursesResponse>.Success(
                    response,
                    "Student courses retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching courses for student {StudentId}", request.StudentId);
                return Result<StudentCoursesResponse>.Failure(ex.Message);
            }
        }
    }
}