using Academic.Application.DTOs;
using Academic.Domain.Aggregates;
using Academic.Domain.Interfaces;
using AutoMapper;
using Core.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;
namespace Academic.Application.Commands.Courses;
public class RecordGradeCommand : IRequest<Result<GradeResponse>>
{
    public RecordGradeRequest Request { get; set; }
    public RecordGradeCommand(RecordGradeRequest request)
    {
        Request = request ?? throw new ArgumentNullException(nameof(request));
    }
    public class Handler : IRequestHandler<RecordGradeCommand, Result<GradeResponse>>
    {
        private readonly IGradeRepository _gradeRepository;
        private readonly ICourseRegistrationRepository _registrationRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<Handler> _logger;
        public Handler(
            IGradeRepository gradeRepository,
            ICourseRegistrationRepository registrationRepository,
            IMapper mapper,
            ILogger<Handler> logger)
        {
            _gradeRepository = gradeRepository ?? throw new ArgumentNullException(nameof(gradeRepository));
            _registrationRepository = registrationRepository ?? throw new ArgumentNullException(nameof(registrationRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task<Result<GradeResponse>> Handle(
            RecordGradeCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation(
                    "Recording grade for student {StudentId} in course {CourseId}",
                    request.Request.StudentId,
                    request.Request.CourseId);
                var registration = await _registrationRepository.GetByIdAsync(
                    request.Request.RegistrationId,
                    cancellationToken);
                if (registration == null)
                {
                    _logger.LogWarning(
                        "Registration not found with ID: {RegistrationId}",
                        request.Request.RegistrationId);
                    return Result<GradeResponse>.Failure(
                        $"Registration with ID {request.Request.RegistrationId} not found");
                }
                var grade = Grade.Create(
                    studentId: request.Request.StudentId,
                    courseId: request.Request.CourseId,
                    registrationId: request.Request.RegistrationId,
                    semester: request.Request.Semester,
                    midtermScore: request.Request.MidtermScore,
                    finalScore: request.Request.FinalScore,
                    ects: request.Request.ECTS);
                await _gradeRepository.AddAsync(grade, cancellationToken);
                registration.AssignGrade(grade.Id);
                await _registrationRepository.UpdateAsync(registration, cancellationToken);
                await _gradeRepository.SaveChangesAsync(cancellationToken);
                _logger.LogInformation(
                    "Grade recorded successfully with ID: {GradeId}",
                    grade.Id);
                var response = _mapper.Map<GradeResponse>(grade);
                return Result<GradeResponse>.Success(
                    response,
                    "Grade recorded successfully");
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error while recording grade");
                return Result<GradeResponse>.Failure(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recording grade");
                return Result<GradeResponse>.Failure(ex.Message);
            }
        }
    }
}