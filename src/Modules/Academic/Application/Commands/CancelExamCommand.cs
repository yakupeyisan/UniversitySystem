using Academic.Application.DTOs;
using Academic.Domain.Aggregates;
using Core.Domain.Repositories;
using Core.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Academic.Application.Commands.Courses;

public class CancelExamCommand : IRequest<Result<Unit>>
{
    public CancelExamCommand(CancelExamRequest request)
    {
        Request = request ?? throw new ArgumentNullException(nameof(request));
    }

    public CancelExamRequest Request { get; set; }

    public class Handler : IRequestHandler<CancelExamCommand, Result<Unit>>
    {
        private readonly IRepository<Exam> _examRepository;
        private readonly ILogger<Handler> _logger;

        public Handler(
            IRepository<Exam> examRepository,
            ILogger<Handler> logger)
        {
            _examRepository = examRepository ?? throw new ArgumentNullException(nameof(examRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<Unit>> Handle(
            CancelExamCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation(
                    "Cancelling exam with ID: {ExamId}",
                    request.Request.ExamId);
                var exam = await _examRepository.GetByIdAsync(
                    request.Request.ExamId,
                    cancellationToken);
                if (exam == null)
                {
                    _logger.LogWarning(
                        "Exam not found with ID: {ExamId}",
                        request.Request.ExamId);
                    return Result<Unit>.Failure(
                        $"Exam with ID {request.Request.ExamId} not found");
                }

                exam.Cancel(request.Request.Reason);
                await _examRepository.UpdateAsync(exam, cancellationToken);
                await _examRepository.SaveChangesAsync(cancellationToken);
                _logger.LogInformation(
                    "Exam {ExamId} cancelled successfully",
                    exam.Id);
                return Result<Unit>.Success(Unit.Value, "Exam cancelled successfully");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Business logic error while cancelling exam");
                return Result<Unit>.Failure(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling exam");
                return Result<Unit>.Failure(ex.Message);
            }
        }
    }
}