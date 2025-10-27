using Academic.Application.DTOs;
using Academic.Domain.Interfaces;
using AutoMapper;
using Core.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;
namespace Academic.Application.Queries.Courses;
public class GetPendingGradeObjectionsQuery : IRequest<Result<IEnumerable<GradeObjectionResponse>>>
{
    public GetPendingGradeObjectionsQuery() { }
    public class Handler : IRequestHandler<GetPendingGradeObjectionsQuery, Result<IEnumerable<GradeObjectionResponse>>>
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
            GetPendingGradeObjectionsQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching pending grade objections");
                var objections = await _objectionRepository.GetPendingObjectionsAsync(cancellationToken);
                var responses = _mapper.Map<IEnumerable<GradeObjectionResponse>>(objections);
                _logger.LogInformation("Retrieved {Count} pending grade objections",
                    objections.Count());
                return Result<IEnumerable<GradeObjectionResponse>>.Success(responses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching pending grade objections");
                return Result<IEnumerable<GradeObjectionResponse>>.Failure(ex.Message);
            }
        }
    }
}