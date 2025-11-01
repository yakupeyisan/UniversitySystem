using Academic.Application.DTOs;
using Academic.Domain.Aggregates;
using Academic.Domain.Enums;
using Core.Domain.Repositories;
using Core.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;
namespace Academic.Application.Commands.Courses;
public class ApproveGradeObjectionCommand : IRequest<Result<Unit>>
{
    public ApproveGradeObjectionCommand(ApproveGradeObjectionRequest request)
    {
        Request = request ?? throw new ArgumentNullException(nameof(request));
    }
    public ApproveGradeObjectionRequest Request { get; set; }
    public class Handler : IRequestHandler<ApproveGradeObjectionCommand, Result<Unit>>
    {
        private readonly IRepository<Grade> _gradeRepository;
        private readonly ILogger<Handler> _logger;
        private readonly IRepository<GradeObjection> _objectionRepository;
        public Handler(
            IRepository<GradeObjection> objectionRepository,
            IRepository<Grade> gradeRepository,
            ILogger<Handler> logger)
        {
            _objectionRepository = objectionRepository ?? throw new ArgumentNullException(nameof(objectionRepository));
            _gradeRepository = gradeRepository ?? throw new ArgumentNullException(nameof(gradeRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task<Result<Unit>> Handle(
            ApproveGradeObjectionCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation(
                    "Approving grade objection {ObjectionId} by reviewer {ReviewedBy}",
                    request.Request.ObjectionId,
                    request.Request.ReviewedBy);
                var objection = await _objectionRepository.GetByIdAsync(
                    request.Request.ObjectionId,
                    cancellationToken);
                if (objection == null)
                {
                    _logger.LogWarning(
                        "Grade objection not found with ID: {ObjectionId}",
                        request.Request.ObjectionId);
                    return Result<Unit>.Failure(
                        $"Grade objection with ID {request.Request.ObjectionId} not found");
                }
                objection.Approve(
                    request.Request.ReviewedBy,
                    request.Request.ReviewNotes,
                    request.Request.NewScore,
                    request.Request.NewLetterGrade.HasValue
                        ? (LetterGrade)request.Request.NewLetterGrade.Value
                        : null);
                if (request.Request.NewScore.HasValue)
                {
                    var grade = await _gradeRepository.GetByIdAsync(
                        objection.GradeId,
                        cancellationToken);
                    if (grade != null)
                    {
                        var letterGrade = request.Request.NewLetterGrade.HasValue
                            ? (LetterGrade)request.Request.NewLetterGrade.Value
                            : grade.LetterGrade;
                        grade.UpdateGradeFromObjection(
                            grade.MidtermScore,
                            request.Request.NewScore.Value,
                            letterGrade);
                        await _gradeRepository.UpdateAsync(grade, cancellationToken);
                    }
                }
                await _objectionRepository.UpdateAsync(objection, cancellationToken);
                await _objectionRepository.SaveChangesAsync(cancellationToken);
                _logger.LogInformation(
                    "Grade objection {ObjectionId} approved successfully",
                    objection.Id);
                return Result<Unit>.Success(Unit.Value, "Grade objection approved successfully");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Business logic error while approving grade objection");
                return Result<Unit>.Failure(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving grade objection");
                return Result<Unit>.Failure(ex.Message);
            }
        }
    }
}