using Academic.Application.DTOs;
using Academic.Domain.Enums;
using AutoMapper;
using Core.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Academic.Application.Commands.Courses;

public class RejectGradeObjectionCommand : IRequest<Result<GradeObjectionResponse>>
{
    public RejectGradeObjectionCommand(Guid objectionId, RejectGradeObjectionRequest request)
    {
        if (objectionId == Guid.Empty)
            throw new ArgumentException("Objection ID cannot be empty", nameof(objectionId));
        ObjectionId = objectionId;
        Request = request ?? throw new ArgumentNullException(nameof(request));
    }

    public Guid ObjectionId { get; set; }
    public RejectGradeObjectionRequest Request { get; set; }

    public class Handler : IRequestHandler<RejectGradeObjectionCommand, Result<GradeObjectionResponse>>
    {
        private readonly ILogger<Handler> _logger;
        private readonly IMapper _mapper;
        private readonly IGradeObjectionRepository _objectionRepository;

        public Handler(
            IGradeObjectionRepository objectionRepository,
            IMapper mapper,
            ILogger<Handler> logger)
        {
            _objectionRepository = objectionRepository ?? throw new ArgumentNullException(nameof(objectionRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<GradeObjectionResponse>> Handle(
            RejectGradeObjectionCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation(
                    "Rejecting grade objection {ObjectionId}",
                    request.ObjectionId);
                var objection = await _objectionRepository.GetByIdAsync(
                    request.ObjectionId,
                    cancellationToken);
                if (objection == null)
                {
                    _logger.LogWarning(
                        "Grade objection not found with ID: {ObjectionId}",
                        request.ObjectionId);
                    return Result<GradeObjectionResponse>.Failure(
                        $"Grade objection with ID {request.ObjectionId} not found");
                }

                if (objection.Status != GradeObjectionStatus.UnderReview &&
                    objection.Status != GradeObjectionStatus.Escalated)
                {
                    _logger.LogWarning(
                        "Grade objection {ObjectionId} cannot be rejected. Current status: {Status}",
                        request.ObjectionId,
                        objection.Status);
                    return Result<GradeObjectionResponse>.Failure(
                        $"Grade objection cannot be rejected. Current status: {objection.Status}");
                }

                objection.Reject(
                    reviewedBy: request.Request.ReviewedBy,
                    notes: request.Request.RejectionReason);
                await _objectionRepository.UpdateAsync(objection, cancellationToken);
                await _objectionRepository.SaveChangesAsync(cancellationToken);
                _logger.LogInformation(
                    "Grade objection {ObjectionId} rejected successfully by reviewer {ReviewedBy}",
                    objection.Id,
                    request.Request.ReviewedBy);
                var response = _mapper.Map<GradeObjectionResponse>(objection);
                return Result<GradeObjectionResponse>.Success(
                    response,
                    "Grade objection rejected successfully");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Business logic error while rejecting grade objection");
                return Result<GradeObjectionResponse>.Failure(ex.Message);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error while rejecting grade objection");
                return Result<GradeObjectionResponse>.Failure(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting grade objection {ObjectionId}", request.ObjectionId);
                return Result<GradeObjectionResponse>.Failure(ex.Message);
            }
        }
    }
}