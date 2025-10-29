using Academic.Application.DTOs;
using Academic.Domain.Aggregates;
using Academic.Domain.Specifications;
using AutoMapper;
using Core.Domain.Repositories;
using Core.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Academic.Application.Commands.Courses;

public class EnrollStudentCommand : IRequest<Result<CourseRegistrationResponse>>
{
    public EnrollStudentCommand(EnrollStudentRequest request)
    {
        Request = request ?? throw new ArgumentNullException(nameof(request));
    }

    public EnrollStudentRequest Request { get; set; }

    public class Handler : IRequestHandler<EnrollStudentCommand, Result<CourseRegistrationResponse>>
    {
        private readonly IRepository<Course> _courseRepository;
        private readonly ILogger<Handler> _logger;
        private readonly IMapper _mapper;
        private readonly IRepository<CourseRegistration> _registrationRepository;

        public Handler(
            IRepository<CourseRegistration> registrationRepository,
            IRepository<Course> courseRepository,
            IMapper mapper,
            ILogger<Handler> logger)
        {
            _registrationRepository =
                registrationRepository ?? throw new ArgumentNullException(nameof(registrationRepository));
            _courseRepository = courseRepository ?? throw new ArgumentNullException(nameof(courseRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<CourseRegistrationResponse>> Handle(
            EnrollStudentCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation(
                    "Enrolling student {StudentId} in course {CourseId}",
                    request.Request.StudentId,
                    request.Request.CourseId);
                var existingRegistration = await _registrationRepository.GetAsync(
                    new CourseRegistrationByStudentAndCourseSpec(request.Request.StudentId, request.Request.CourseId),
                    cancellationToken);
                if (existingRegistration != null)
                {
                    _logger.LogWarning(
                        "Student {StudentId} is already registered for course {CourseId}",
                        request.Request.StudentId,
                        request.Request.CourseId);
                    return Result<CourseRegistrationResponse>.Failure(
                        "Student is already registered for this course");
                }

                var course = await _courseRepository.GetByIdAsync(
                    request.Request.CourseId,
                    cancellationToken);
                if (course == null)
                {
                    _logger.LogWarning(
                        "Course not found with ID: {CourseId}",
                        request.Request.CourseId);
                    return Result<CourseRegistrationResponse>.Failure(
                        $"Course with ID {request.Request.CourseId} not found");
                }

                var registration = CourseRegistration.Create(
                    request.Request.StudentId,
                    request.Request.CourseId,
                    request.Request.Semester,
                    request.Request.IsRetake,
                    request.Request.PreviousGradeId);
                await _registrationRepository.AddAsync(registration, cancellationToken);
                await _registrationRepository.SaveChangesAsync(cancellationToken);
                _logger.LogInformation(
                    "Student {StudentId} enrolled in course {CourseId} successfully",
                    request.Request.StudentId,
                    request.Request.CourseId);
                var response = _mapper.Map<CourseRegistrationResponse>(registration);
                return Result<CourseRegistrationResponse>.Success(
                    response,
                    "Student enrolled successfully");
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error while enrolling student");
                return Result<CourseRegistrationResponse>.Failure(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enrolling student");
                return Result<CourseRegistrationResponse>.Failure(ex.Message);
            }
        }
    }
}