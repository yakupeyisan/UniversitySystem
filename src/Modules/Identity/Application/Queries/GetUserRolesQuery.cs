using AutoMapper;
using Core.Domain.Repositories;
using Core.Domain.Results;
using Identity.Application.DTOs;
using Identity.Domain.Aggregates;
using Identity.Domain.Specifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Queries;

/// <summary>
/// Kullanýcýnýn rollerini al
/// </summary>
public class GetUserRolesQuery : IRequest<Result<List<RoleDto>>>
{
    public GetUserRolesQuery(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        UserId = userId;
    }

    public Guid UserId { get; set; }

    public class Handler : IRequestHandler<GetUserRolesQuery, Result<List<RoleDto>>>
    {
        private readonly ILogger<Handler> _logger;
        private readonly IRepository<User> _userRepository;
        private readonly AutoMapper.IMapper _mapper;

        public Handler(
            IRepository<User> userRepository,
            AutoMapper.IMapper mapper,
            ILogger<Handler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<List<RoleDto>>> Handle(
            GetUserRolesQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Getting roles for user {UserId}", request.UserId);

                var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
                if (user == null)
                {
                    _logger.LogWarning("User not found: {UserId}", request.UserId);
                    return Result<List<RoleDto>>.Failure("User not found");
                }

                var roleDtos = _mapper.Map<List<RoleDto>>(user.Roles);

                _logger.LogInformation("Retrieved {Count} roles for user {UserId}",
                    roleDtos.Count, request.UserId);

                return Result<List<RoleDto>>.Success(roleDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting roles for user {UserId}", request.UserId);
                return Result<List<RoleDto>>.Failure("An error occurred while retrieving roles");
            }
        }
    }
}

/// <summary>
/// Kullanýcýnýn giriþ geçmiþini al
/// </summary>
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

/// <summary>
/// Hesap kilitleme durumunu kontrol et
/// </summary>
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

                // Aktif kilitlemeleri bul
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