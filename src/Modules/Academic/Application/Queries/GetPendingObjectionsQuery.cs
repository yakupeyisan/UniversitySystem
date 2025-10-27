using Academic.Application.DTOs;
using Academic.Domain.Interfaces;
using AutoMapper;
using Core.Domain.Pagination;
using Core.Domain.Results;
using MediatR;
using Microsoft.Extensions.Logging;
namespace Academic.Application.Queries.Courses;
public class GetPendingObjectionsQuery : IRequest<Result<List<GradeObjectionResponse>>>
{
    public PagedRequest? PagedRequest { get; set; }
    public GetPendingObjectionsQuery(PagedRequest? pagedRequest = null)
    {
        PagedRequest = pagedRequest;
    }
    public class Handler : IRequestHandler<GetPendingObjectionsQuery, Result<List<GradeObjectionResponse>>>
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
        public async Task<Result<List<GradeObjectionResponse>>> Handle(
            GetPendingObjectionsQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching pending grade objections");
                var objections = await _objectionRepository.GetPendingObjectionsAsync(cancellationToken);
                var responses = _mapper.Map<List<GradeObjectionResponse>>(objections);
                _logger.LogInformation(
                    "Retrieved {Count} pending grade objections",
                    objections.Count());
                return Result<List<GradeObjectionResponse>>.Success(
                    responses,
                    "Pending objections retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching pending grade objections");
                return Result<List<GradeObjectionResponse>>.Failure(ex.Message);
            }
        }
    }
}