using Academic.Application.DTOs;
using Academic.Domain.Interfaces;
using AutoMapper;
using Core.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Academic.Application.Commands.Courses;

/// <summary>
/// Command to reject a grade objection
/// </summary>
public class RejectGradeObjectionCommand : IRequest<Result<GradeObjectionResponse>>
{
    public Guid ObjectionId { get; set; }
    public RejectGradeObjectionRequest Request { get; set; }

    public RejectGradeObjectionCommand(Guid objectionId, RejectGradeObjectionRequest request)
    {
        if (objectionId == Guid.Empty)
            throw new ArgumentException("Objection ID cannot be empty", nameof(objectionId));

        ObjectionId = objectionId;
        Request = request ?? throw new ArgumentNullException(nameof(request));
    }

    public class Handler : IRequestHandler<RejectGradeObjectionCommand, Result<GradeObjectionResponse>>
    {
        private readonly IGradeObjectionRepository _objectionRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<Handler> _logger;

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

                // Get the objection
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

                // Check if objection is in a state that can be rejected
                if (objection.Status != Domain.Enums.GradeObjectionStatus.UnderReview &&
                    objection.Status != Domain.Enums.GradeObjectionStatus.Escalated)
                {
                    _logger.LogWarning(
                        "Grade objection {ObjectionId} cannot be rejected. Current status: {Status}",
                        request.ObjectionId,
                        objection.Status);
                    return Result<GradeObjectionResponse>.Failure(
                        $"Grade objection cannot be rejected. Current status: {objection.Status}");
                }

                // Reject the objection
                objection.Reject(
                    reviewedBy: request.Request.ReviewedBy,
                    notes: request.Request.RejectionReason);

                // Update in database
                await _objectionRepository.UpdateAsync(objection, cancellationToken);
                await _objectionRepository.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Grade objection {ObjectionId} rejected successfully by reviewer {ReviewedBy}",
                    objection.Id,
                    request.Request.ReviewedBy);

                // Map to response
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