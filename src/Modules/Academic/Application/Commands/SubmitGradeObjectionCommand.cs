using Academic.Application.DTOs;
using Academic.Domain.Aggregates;
using AutoMapper;
using Core.Domain.Repositories;
using Core.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;
namespace Academic.Application.Commands.Courses;
public class SubmitGradeObjectionCommand : IRequest<Result<GradeObjectionResponse>>
{
    public SubmitGradeObjectionCommand(SubmitGradeObjectionRequest request)
    {
        Request = request ?? throw new ArgumentNullException(nameof(request));
    }
    public SubmitGradeObjectionRequest Request { get; set; }
    public class Handler : IRequestHandler<SubmitGradeObjectionCommand, Result<GradeObjectionResponse>>
    {
        private readonly IRepository<Grade> _gradeRepository;
        private readonly ILogger<Handler> _logger;
        private readonly IMapper _mapper;
        private readonly IRepository<GradeObjection> _objectionRepository;
        public Handler(
            IRepository<GradeObjection> objectionRepository,
            IRepository<Grade> gradeRepository,
            IMapper mapper,
            ILogger<Handler> logger)
        {
            _objectionRepository = objectionRepository ?? throw new ArgumentNullException(nameof(objectionRepository));
            _gradeRepository = gradeRepository ?? throw new ArgumentNullException(nameof(gradeRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task<Result<GradeObjectionResponse>> Handle(
            SubmitGradeObjectionCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation(
                    "Submitting grade objection for student {StudentId} on grade {GradeId}",
                    request.Request.StudentId,
                    request.Request.GradeId);
                var grade = await _gradeRepository.GetByIdAsync(
                    request.Request.GradeId,
                    cancellationToken);
                if (grade == null)
                {
                    _logger.LogWarning(
                        "Grade not found with ID: {GradeId}",
                        request.Request.GradeId);
                    return Result<GradeObjectionResponse>.Failure(
                        $"Grade with ID {request.Request.GradeId} not found");
                }
                var objection = GradeObjection.Create(
                    request.Request.GradeId,
                    request.Request.StudentId,
                    request.Request.CourseId,
                    request.Request.Reason);
                await _objectionRepository.AddAsync(objection, cancellationToken);
                await _objectionRepository.SaveChangesAsync(cancellationToken);
                _logger.LogInformation(
                    "Grade objection submitted successfully with ID: {ObjectionId}",
                    objection.Id);
                var response = _mapper.Map<GradeObjectionResponse>(objection);
                return Result<GradeObjectionResponse>.Success(
                    response,
                    "Grade objection submitted successfully");
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error while submitting grade objection");
                return Result<GradeObjectionResponse>.Failure(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting grade objection");
                return Result<GradeObjectionResponse>.Failure(ex.Message);
            }
        }
    }
}