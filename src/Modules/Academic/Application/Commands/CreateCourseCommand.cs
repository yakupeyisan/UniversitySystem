
using Academic.Application.DTOs;
using Academic.Domain.Aggregates;
using Academic.Domain.Enums;
using Academic.Domain.Interfaces;
using Academic.Domain.ValueObjects;
using AutoMapper;
using Core.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Academic.Application.Commands.Courses;

/// <summary>
/// Command to create a new course
/// </summary>
public class CreateCourseCommand : IRequest<Result<CourseResponse>>
{
    public CreateCourseRequest Request { get; set; }

    public CreateCourseCommand(CreateCourseRequest request)
    {
        Request = request ?? throw new ArgumentNullException(nameof(request));
    }

    /// <summary>
    /// Handler for CreateCourseCommand
    /// </summary>
    public class Handler : IRequestHandler<CreateCourseCommand, Result<CourseResponse>>
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

        public async Task<Result<CourseResponse>> Handle(
            CreateCourseCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation(
                    "Creating new course: {CourseCode} - {CourseName}",
                    request.Request.CourseCode,
                    request.Request.Name);

                // Check if course code already exists
                var existingCourse = await _courseRepository.GetByCourseCodeAsync(
                    request.Request.CourseCode,
                    cancellationToken);

                if (existingCourse != null)
                {
                    _logger.LogWarning(
                        "Course with code {CourseCode} already exists",
                        request.Request.CourseCode);
                    return Result<CourseResponse>.Failure(
                        $"Course with code {request.Request.CourseCode} already exists");
                }

                // Create course code value object
                var courseCode = CourseCode.Create(request.Request.CourseCode);

                // Create capacity info value object
                var capacityInfo = CapacityInfo.Create(request.Request.MaxCapacity);

                // Create course aggregate
                var course = Course.Create(
                    code: courseCode,
                    name: request.Request.Name,
                    description: request.Request.Description,
                    ects: request.Request.ECTS,
                    credits: request.Request.Credits,
                    level: (CourseLevel)request.Request.Level,
                    type: (CourseType)request.Request.Type,
                    semester: (CourseSemester)request.Request.Semester,
                    year: request.Request.Year,
                    departmentId: request.Request.DepartmentId,
                    capacityInfo: capacityInfo);

                // Save to database
                await _courseRepository.AddAsync(course, cancellationToken);
                await _courseRepository.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Course created successfully with ID: {CourseId}",
                    course.Id);

                var response = _mapper.Map<CourseResponse>(course);
                return Result<CourseResponse>.Success(
                    response,
                    "Course created successfully");
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error while creating course");
                return Result<CourseResponse>.Failure(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating course");
                return Result<CourseResponse>.Failure(ex.Message);
            }
        }
    }
}

// ============================================================================
// FILE: src/Modules/Academic/Application/Commands/Exams/PostponeExamCommand.cs
// ============================================================================

// ============================================================================
// FILE: src/Modules/Academic/Application/Commands/Exams/CancelExamCommand.cs
// ============================================================================

// ============================================================================
// FILE: src/Modules/Academic/Application/Commands/Enrollments/DropCourseCommand.cs
// ============================================================================

// ============================================================================
// FILE: src/Modules/Academic/Application/Commands/Enrollments/JoinWaitingListCommand.cs
// ============================================================================

// ============================================================================
// FILE: src/Modules/Academic/Application/Commands/Grades/UpdateGradeCommand.cs
// ============================================================================

// ============================================================================
// FILE: src/Modules/Academic/Application/Commands/GradeObjections/SubmitGradeObjectionCommand.cs
// ============================================================================

// ============================================================================
// FILE: src/Modules/Academic/Application/Commands/GradeObjections/ApproveGradeObjectionCommand.cs
// ============================================================================