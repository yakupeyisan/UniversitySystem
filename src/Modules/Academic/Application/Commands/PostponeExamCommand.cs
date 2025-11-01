using Academic.Application.DTOs;
using Academic.Domain.Aggregates;
using Academic.Domain.ValueObjects;
using AutoMapper;
using Core.Domain.Repositories;
using Core.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;
namespace Academic.Application.Commands.Courses;
public class PostponeExamCommand : IRequest<Result<ExamResponse>>
{
    public PostponeExamCommand(PostponeExamRequest request)
    {
        Request = request ?? throw new ArgumentNullException(nameof(request));
    }
    public PostponeExamRequest Request { get; set; }
    public class Handler : IRequestHandler<PostponeExamCommand, Result<ExamResponse>>
    {
        private readonly IRepository<Exam> _examRepository;
        private readonly ILogger<Handler> _logger;
        private readonly IMapper _mapper;
        public Handler(
            IRepository<Exam> examRepository,
            IMapper mapper,
            ILogger<Handler> logger)
        {
            _examRepository = examRepository ?? throw new ArgumentNullException(nameof(examRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task<Result<ExamResponse>> Handle(
            PostponeExamCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation(
                    "Postponing exam with ID: {ExamId}",
                    request.Request.ExamId);
                var exam = await _examRepository.GetByIdAsync(
                    request.Request.ExamId,
                    cancellationToken);
                if (exam == null)
                {
                    _logger.LogWarning(
                        "Exam not found with ID: {ExamId}",
                        request.Request.ExamId);
                    return Result<ExamResponse>.Failure(
                        $"Exam with ID {request.Request.ExamId} not found");
                }
                if (!DateOnly.TryParse(request.Request.NewExamDate, out var newDate))
                    return Result<ExamResponse>.Failure("Invalid exam date format (yyyy-MM-dd)");
                var newTimeSlot = TimeSlot.Create(
                    request.Request.NewStartTime,
                    request.Request.NewEndTime);
                exam.Postpone(newDate, newTimeSlot);
                await _examRepository.UpdateAsync(exam, cancellationToken);
                await _examRepository.SaveChangesAsync(cancellationToken);
                _logger.LogInformation(
                    "Exam {ExamId} postponed successfully to {NewDate}",
                    exam.Id,
                    newDate);
                var response = _mapper.Map<ExamResponse>(exam);
                return Result<ExamResponse>.Success(
                    response,
                    "Exam postponed successfully");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Business logic error while postponing exam");
                return Result<ExamResponse>.Failure(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error postponing exam");
                return Result<ExamResponse>.Failure(ex.Message);
            }
        }
    }
}