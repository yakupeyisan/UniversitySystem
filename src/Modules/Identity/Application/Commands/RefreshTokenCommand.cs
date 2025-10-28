using System.Security.Claims;
using Core.Domain.Results;
using Identity.Application.Abstractions;
using Identity.Application.DTOs;
using Identity.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Commands;

public class RefreshTokenCommand : IRequest<Result<TokenResponse>>
{
    public string RefreshToken { get; set; } = string.Empty;

    public RefreshTokenCommand(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            throw new ArgumentException("Refresh token cannot be empty", nameof(refreshToken));

        RefreshToken = refreshToken.Trim();
    }

    public class Handler : IRequestHandler<RefreshTokenCommand, Result<TokenResponse>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly ILogger<Handler> _logger;

        public Handler(
            IUserRepository userRepository,
            ITokenService tokenService,
            ILogger<Handler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<TokenResponse>> Handle(
            RefreshTokenCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Refreshing token");

                var principal = _tokenService.GetPrincipalFromExpiredToken(request.RefreshToken);
                if (principal == null)
                    return Result<TokenResponse>.Failure("Invalid refresh token");

                var userId = Guid.Parse(principal.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
                var user = await _userRepository.GetByIdAsync(userId, cancellationToken);

                if (user == null || user.RefreshToken != request.RefreshToken)
                {
                    _logger.LogWarning("Invalid refresh token for user: {UserId}", userId);
                    return Result<TokenResponse>.Failure("Invalid refresh token");
                }

                var token = _tokenService.GenerateAccessToken(user);
                var newRefreshToken = _tokenService.GenerateRefreshToken();

                user.UpdateRefreshToken(newRefreshToken);
                await _userRepository.UpdateAsync(user, cancellationToken);
                await _userRepository.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Token refreshed successfully for user: {UserId}", userId);
                return Result<TokenResponse>.Success(new TokenResponse
                {
                    AccessToken = token,
                    RefreshToken = newRefreshToken
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing token");
                return Result<TokenResponse>.Failure("An unexpected error occurred while refreshing token");
            }
        }
    }
}