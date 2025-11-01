using Core.Domain.Repositories;
using Core.Domain.Results;
using Identity.Application.DTOs;
using Identity.Domain.Aggregates;
using Identity.Domain.Specifications;
using MediatR;
using Microsoft.Extensions.Logging;
namespace Identity.Application.Queries;
public class GetAccountLockoutStatusQuery : IRequest<Result<AccountLockoutStatusDto>>
{
    public GetAccountLockoutStatusQuery(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));
        UserId = userId;
    }
    public Guid UserId { get; set; }
    public class Handler : IRequestHandler<GetAccountLockoutStatusQuery, Result<AccountLockoutStatusDto>>
    {
        private readonly ILogger<Handler> _logger;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<UserAccountLockout> _lockoutRepository;
        public Handler(
            IRepository<User> userRepository,
            IRepository<UserAccountLockout> lockoutRepository,
            ILogger<Handler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _lockoutRepository = lockoutRepository ?? throw new ArgumentNullException(nameof(lockoutRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task<Result<AccountLockoutStatusDto>> Handle(
            GetAccountLockoutStatusQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Getting lockout status for user {UserId}", request.UserId);
                var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
                if (user == null)
                {
                    _logger.LogWarning("User not found: {UserId}", request.UserId);
                    return Result<AccountLockoutStatusDto>.Failure("User not found");
                }
                var spec = new ActiveLockoutsSpecification(request.UserId);
                var lockouts = await _lockoutRepository.GetAllAsync(spec, cancellationToken);
                var activeLockout = lockouts.FirstOrDefault();
                var status = new AccountLockoutStatusDto
                {
                    UserId = request.UserId,
                    IsLocked = user.IsAccountLocked,
                    ActiveLockoutCount = lockouts.Count(),
                    FailedAttempts = user.FailedLoginAttemptCount
                };
                if (activeLockout != null && activeLockout.IsActiveLockout())
                {
                    status.IsCurrentlyLocked = true;
                    status.LockedUntil = activeLockout.LockedUntil;
                    status.RemainingMinutes = activeLockout.GetRemainingLockoutMinutes();
                    status.LockReason = activeLockout.Reason.ToString();
                }
                return Result<AccountLockoutStatusDto>.Success(status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting lockout status for user {UserId}", request.UserId);
                return Result<AccountLockoutStatusDto>.Failure("An error occurred while retrieving lockout status");
            }
        }
    }
}