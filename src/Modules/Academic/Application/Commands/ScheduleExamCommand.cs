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
public class ScheduleExamCommand : IRequest<Result<ExamResponse>>
{
    public ScheduleExamRequest Request { get; set; }
    public ScheduleExamCommand(ScheduleExamRequest request)
    {
        Request = request ?? throw new ArgumentNullException(nameof(request));
    }
    public class Handler : IRequestHandler<ScheduleExamCommand, Result<ExamResponse>>
    {
        private readonly IExamRepository _examRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<Handler> _logger;
        public Handler(
            IExamRepository examRepository,
            ICourseRepository courseRepository,
            IMapper mapper,
            ILogger<Handler> logger)
        {
            _examRepository = examRepository ?? throw new ArgumentNullException(nameof(examRepository));
            _courseRepository = courseRepository ?? throw new ArgumentNullException(nameof(courseRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task<Result<ExamResponse>> Handle(
            ScheduleExamCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation(
                    "Scheduling exam for course: {CourseId}",
                    request.Request.CourseId);
                var course = await _courseRepository.GetByIdAsync(
                    request.Request.CourseId,
                    cancellationToken);
                if (course == null)
                {
                    _logger.LogWarning(
                        "Course not found with ID: {CourseId}",
                        request.Request.CourseId);
                    return Result<ExamResponse>.Failure(
                        $"Course with ID {request.Request.CourseId} not found");
                }
                if (!DateOnly.TryParse(request.Request.ExamDate, out var examDate))
                {
                    return Result<ExamResponse>.Failure("Invalid exam date format (yyyy-MM-dd)");
                }
                var timeSlot = TimeSlot.Create(
                    request.Request.StartTime,
                    request.Request.EndTime);
                var exam = Exam.Create(
                    courseId: request.Request.CourseId,
                    examType: (ExamType)request.Request.ExamType,
                    examDate: examDate,
                    timeSlot: timeSlot,
                    maxCapacity: request.Request.MaxCapacity,
                    examRoomId: request.Request.ExamRoomId,
                    isOnline: request.Request.IsOnline,
                    onlineLink: request.Request.OnlineLink);
                await _examRepository.AddAsync(exam, cancellationToken);
                await _examRepository.SaveChangesAsync(cancellationToken);
                _logger.LogInformation(
                    "Exam scheduled successfully with ID: {ExamId}",
                    exam.Id);
                var response = _mapper.Map<ExamResponse>(exam);
                return Result<ExamResponse>.Success(
                    response,
                    "Exam scheduled successfully");
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error while scheduling exam");
                return Result<ExamResponse>.Failure(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error scheduling exam");
                return Result<ExamResponse>.Failure(ex.Message);
            }
        }
    }
}