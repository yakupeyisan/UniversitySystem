using Core.Domain.Repositories;
using Core.Domain.Results;
using Identity.Application.DTOs;
using Identity.Domain.Aggregates;
using Identity.Domain.Specifications;
using MediatR;
using Microsoft.Extensions.Logging;
namespace Identity.Application.Queries;
public class GetLoginHistoryQuery : IRequest<Result<List<LoginHistoryDto>>>
{
    public GetLoginHistoryQuery(Guid userId, int days = 30, int limit = 100)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));
        if (days <= 0)
            throw new ArgumentException("Days must be greater than 0", nameof(days));
        UserId = userId;
        Days = days;
        Limit = limit;
    }
    public Guid UserId { get; set; }
    public int Days { get; set; }
    public int Limit { get; set; }
    public class Handler : IRequestHandler<GetLoginHistoryQuery, Result<List<LoginHistoryDto>>>
    {
        private readonly ILogger<Handler> _logger;
        private readonly IRepository<LoginHistory> _loginHistoryRepository;
        private readonly AutoMapper.IMapper _mapper;
        public Handler(
            IRepository<LoginHistory> loginHistoryRepository,
            AutoMapper.IMapper mapper,
            ILogger<Handler> logger)
        {
            _loginHistoryRepository =
                loginHistoryRepository ?? throw new ArgumentNullException(nameof(loginHistoryRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task<Result<List<LoginHistoryDto>>> Handle(
            GetLoginHistoryQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Getting login history for user {UserId} for last {Days} days",
                    request.UserId, request.Days);
                var cutoffDate = DateTime.UtcNow.AddDays(-request.Days);
                var spec = new ActiveLoginHistoriesSpecification(request.UserId);
                var loginHistories = await _loginHistoryRepository.GetAllAsync(spec, cancellationToken);
                var limited = loginHistories.Take(request.Limit).ToList();
                var dtos = _mapper.Map<List<LoginHistoryDto>>(limited);
                _logger.LogInformation("Retrieved {Count} login records for user {UserId}",
                    dtos.Count, request.UserId);
                return Result<List<LoginHistoryDto>>.Success(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting login history for user {UserId}", request.UserId);
                return Result<List<LoginHistoryDto>>.Failure("An error occurred while retrieving login history");
            }
        }
    }
}