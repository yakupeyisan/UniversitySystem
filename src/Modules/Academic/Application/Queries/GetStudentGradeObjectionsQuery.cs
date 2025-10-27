using Academic.Application.DTOs;
using Academic.Domain.Interfaces;
using AutoMapper;
using Core.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;
namespace Academic.Application.Queries.Courses;
public class GetStudentGradeObjectionsQuery : IRequest<Result<IEnumerable<GradeObjectionResponse>>>
{
    public Guid StudentId { get; set; }
    public GetStudentGradeObjectionsQuery(Guid studentId)
    {
        if (studentId == Guid.Empty)
            throw new ArgumentException("Student ID cannot be empty", nameof(studentId));
        StudentId = studentId;
    }
    public class Handler : IRequestHandler<GetStudentGradeObjectionsQuery, Result<IEnumerable<GradeObjectionResponse>>>
    {
        private readonly IGradeObjectionRepository _objectionRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<Handler> _logger;
        public Handler(IGradeObjectionRepository objectionRepository, IMapper mapper, ILogger<Handler> logger)
        {
            _objectionRepository = objectionRepository ?? throw new ArgumentNullException(nameof(objectionRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task<Result<IEnumerable<GradeObjectionResponse>>> Handle(
            GetStudentGradeObjectionsQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching grade objections for student: {StudentId}",
                    request.StudentId);
                var objections = await _objectionRepository.GetByStudentAsync(
                    request.StudentId, cancellationToken);
                var responses = _mapper.Map<IEnumerable<GradeObjectionResponse>>(objections);
                _logger.LogInformation("Retrieved {Count} grade objections for student",
                    objections.Count());
                return Result<IEnumerable<GradeObjectionResponse>>.Success(responses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching grade objections for student");
                return Result<IEnumerable<GradeObjectionResponse>>.Failure(ex.Message);
            }
        }
    }
}