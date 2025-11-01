using Academic.Application.DTOs;
using Academic.Domain.Aggregates;
using Academic.Domain.Specifications;
using AutoMapper;
using Core.Domain.Repositories;
using Core.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;
namespace Academic.Application.Queries.Courses;
public class GetStudentGradeObjectionsQuery : IRequest<Result<IEnumerable<GradeObjectionResponse>>>
{
    public GetStudentGradeObjectionsQuery(Guid studentId)
    {
        if (studentId == Guid.Empty)
            throw new ArgumentException("Student ID cannot be empty", nameof(studentId));
        StudentId = studentId;
    }
    public Guid StudentId { get; set; }
    public class Handler : IRequestHandler<GetStudentGradeObjectionsQuery, Result<IEnumerable<GradeObjectionResponse>>>
    {
        private readonly ILogger<Handler> _logger;
        private readonly IMapper _mapper;
        private readonly IRepository<GradeObjection>
            _objectionRepository;
        public Handler(IRepository<GradeObjection>
            objectionRepository, IMapper mapper, ILogger<Handler> logger)
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
                var objections = await _objectionRepository.GetAllAsync(
                    new GradeObjectionsByStudentSpec(request.StudentId), cancellationToken);
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