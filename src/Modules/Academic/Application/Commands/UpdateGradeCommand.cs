using Academic.Application.DTOs;
using Academic.Domain.Aggregates;
using AutoMapper;
using Core.Domain.Repositories;
using Core.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Academic.Application.Commands.Courses;

public class UpdateGradeCommand : IRequest<Result<GradeResponse>>
{
    public UpdateGradeCommand(UpdateGradeRequest request)
    {
        Request = request ?? throw new ArgumentNullException(nameof(request));
    }

    public UpdateGradeRequest Request { get; set; }

    public class Handler : IRequestHandler<UpdateGradeCommand, Result<GradeResponse>>
    {
        private readonly IRepository<Grade> _gradeRepository;
        private readonly ILogger<Handler> _logger;
        private readonly IMapper _mapper;

        public Handler(
            IRepository<Grade> gradeRepository,
            IMapper mapper,
            ILogger<Handler> logger)
        {
            _gradeRepository = gradeRepository ?? throw new ArgumentNullException(nameof(gradeRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<GradeResponse>> Handle(
            UpdateGradeCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation(
                    "Updating grade with ID: {GradeId}",
                    request.Request.GradeId);
                var grade = await _gradeRepository.GetByIdAsync(
                    request.Request.GradeId,
                    cancellationToken);
                if (grade == null)
                {
                    _logger.LogWarning(
                        "Grade not found with ID: {GradeId}",
                        request.Request.GradeId);
                    return Result<GradeResponse>.Failure(
                        $"Grade with ID {request.Request.GradeId} not found");
                }

                grade.UpdateScores(request.Request.MidtermScore, request.Request.FinalScore);
                await _gradeRepository.UpdateAsync(grade, cancellationToken);
                await _gradeRepository.SaveChangesAsync(cancellationToken);
                _logger.LogInformation(
                    "Grade {GradeId} updated successfully",
                    grade.Id);
                var response = _mapper.Map<GradeResponse>(grade);
                return Result<GradeResponse>.Success(
                    response,
                    "Grade updated successfully");
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error while updating grade");
                return Result<GradeResponse>.Failure(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating grade");
                return Result<GradeResponse>.Failure(ex.Message);
            }
        }
    }
}