using Academic.Application.DTOs;
using Academic.Domain.Aggregates;
using Academic.Domain.Enums;
using Academic.Domain.ValueObjects;
using AutoMapper;
using Core.Domain.Repositories;
using Core.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Academic.Application.Commands.Courses;

public class CreateCourseCommand : IRequest<Result<CourseResponse>>
{
    public CreateCourseCommand(CreateCourseRequest request)
    {
        Request = request ?? throw new ArgumentNullException(nameof(request));
    }

    public CreateCourseRequest Request { get; set; }

    public class Handler : IRequestHandler<CreateCourseCommand, Result<CourseResponse>>
    {
        private readonly IRepository<Course> _courseRepository;
        private readonly ILogger<Handler> _logger;
        private readonly IMapper _mapper;

        public Handler(
            IRepository<Course> courseRepository,
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

                var courseCode = CourseCode.Create(request.Request.CourseCode);
                var capacityInfo = CapacityInfo.Create(request.Request.MaxCapacity);
                var course = Course.Create(
                    courseCode,
                    request.Request.Name,
                    description: request.Request.Description,
                    ects: request.Request.ECTS,
                    credits: request.Request.Credits,
                    level: (CourseLevel)request.Request.Level,
                    type: (CourseType)request.Request.Type,
                    semester: (CourseSemester)request.Request.Semester,
                    year: request.Request.Year,
                    departmentId: request.Request.DepartmentId,
                    maxCapacity: request.Request.MaxCapacity);
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