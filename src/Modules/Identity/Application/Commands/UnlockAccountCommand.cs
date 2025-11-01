using AutoMapper;
using Core.Application.Abstractions;
using Core.Domain.Repositories;
using Core.Domain.Results;
using Identity.Application.DTOs;
using Identity.Domain.Aggregates;
using Identity.Domain.Specifications;
using MediatR;
using Microsoft.Extensions.Logging;
namespace Identity.Application.Commands;
public class UnlockAccountCommand : IRequest<Result<UserDto>>
{
    public UnlockAccountCommand(Guid userId, string unlockReason = "")
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));
        UserId = userId;
        UnlockReason = unlockReason ?? string.Empty;
    }
    public Guid UserId { get; set; }
    public string UnlockReason { get; set; }
    public class Handler : IRequestHandler<UnlockAccountCommand, Result<UserDto>>
    {
        private readonly ILogger<Handler> _logger;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<UserAccountLockout> _lockoutRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        public Handler(
            IRepository<User> userRepository,
            IRepository<UserAccountLockout> lockoutRepository,
            IMapper mapper,
            ILogger<Handler> logger, ICurrentUserService currentUserService)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _lockoutRepository = lockoutRepository ?? throw new ArgumentNullException(nameof(lockoutRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _currentUserService = currentUserService;
        }
        public async Task<Result<UserDto>> Handle(
            UnlockAccountCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Unlocking account for user {UserId}", request.UserId);
                var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
                if (user == null)
                    return Result<UserDto>.Failure("User not found");
                user.UnlockAccount(_currentUserService.UserId);
                await _userRepository.UpdateAsync(user, cancellationToken);
                var spec = new ActiveLockoutsSpecification(request.UserId);
                var lockouts = await _lockoutRepository.GetAllAsync(spec, cancellationToken);
                foreach (var lockout in lockouts)
                {
                    lockout.Unlock(request.UnlockReason);
                    await _lockoutRepository.UpdateAsync(lockout, cancellationToken);
                }
                await _userRepository.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Account unlocked for user {UserId}", request.UserId);
                return Result<UserDto>.Success(_mapper.Map<UserDto>(user));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unlocking account for user {UserId}", request.UserId);
                return Result<UserDto>.Failure("An error occurred while unlocking the account");
            }
        }
    }
}