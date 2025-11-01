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

/// <summary>
/// 2FA'yý devre dýþý býrak
/// </summary>
public class Disable2FACommand : IRequest<Result<UserDto>>
{
    public Disable2FACommand(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        UserId = userId;
    }

    public Guid UserId { get; set; }

    public class Handler : IRequestHandler<Disable2FACommand, Result<UserDto>>
    {
        private readonly ILogger<Handler> _logger;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<TwoFactorToken> _twoFactorRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public Handler(
            IRepository<User> userRepository,
            IRepository<TwoFactorToken> twoFactorRepository,
            IMapper mapper,
            ILogger<Handler> logger, ICurrentUserService currentUserService)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _twoFactorRepository = twoFactorRepository ?? throw new ArgumentNullException(nameof(twoFactorRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _currentUserService = currentUserService;
        }

        public async Task<Result<UserDto>> Handle(
            Disable2FACommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Disabling 2FA for user {UserId}", request.UserId);

                var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
                if (user == null)
                    return Result<UserDto>.Failure("User not found");

                // Tüm aktif 2FA token'larýný devre dýþý býrak
                var spec = new ActiveTwoFactorTokensSpecification(request.UserId);

                var activeTokens = await _twoFactorRepository.GetAllAsync(spec, cancellationToken);
                foreach (var token in activeTokens)
                {
                    token.Disable();
                    await _twoFactorRepository.UpdateAsync(token, cancellationToken);
                }

                // Kullanýcýyý 2FA disabled yap
                user.DisableTwoFactor(_currentUserService.UserId);
                await _userRepository.UpdateAsync(user, cancellationToken);
                await _userRepository.SaveChangesAsync(cancellationToken);


                _logger.LogInformation("2FA disabled for user {UserId}", request.UserId);

                return Result<UserDto>.Success(_mapper.Map<UserDto>(user));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disabling 2FA for user {UserId}", request.UserId);
                return Result<UserDto>.Failure("An error occurred while disabling 2FA");
            }
        }
    }
}