using Core.Domain.Repositories;
using Core.Domain.Results;
using Identity.Application.Abstractions;
using Identity.Application.DTOs;
using Identity.Domain.Aggregates;
using Identity.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Commands;

public class ChangePasswordCommand : IRequest<Result<Unit>>
{
    public ChangePasswordCommand(Guid userId, ChangePasswordRequest request)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        UserId = userId;
        Request = request ?? throw new ArgumentNullException(nameof(request));
    }

    public Guid UserId { get; set; }
    public ChangePasswordRequest Request { get; set; }

    public class Handler : IRequestHandler<ChangePasswordCommand, Result<Unit>>
    {
        private readonly ILogger<Handler> _logger;
        private readonly IPasswordHasher _passwordHasher;

        private readonly IRepository<User>
            _userRepository;

        public Handler(
            IRepository<User>
                userRepository,
            IPasswordHasher passwordHasher,
            ILogger<Handler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<Unit>> Handle(
            ChangePasswordCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Attempting to change password for user: {UserId}", request.UserId);

                // Get user
                var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
                if (user == null)
                {
                    _logger.LogWarning("User not found: {UserId}", request.UserId);
                    return Result<Unit>.Failure("User not found");
                }

                // Verify current password
                var isPasswordValid = _passwordHasher.VerifyPassword(
                    request.Request.CurrentPassword,
                    user.PasswordHash.HashedPassword,
                    user.PasswordHash.Salt);

                if (!isPasswordValid)
                {
                    _logger.LogWarning("Invalid current password for user: {UserId}", request.UserId);
                    return Result<Unit>.Failure("Current password is incorrect");
                }

                // Validate new password strength
                if (!_passwordHasher.ValidatePasswordStrength(request.Request.NewPassword))
                {
                    _logger.LogWarning("New password does not meet strength requirements for user: {UserId}",
                        request.UserId);
                    return Result<Unit>.Failure(
                        $"New password does not meet requirements: {_passwordHasher.GetPasswordRequirements()}");
                }

                // Hash new password
                var (hashedPassword, salt) = _passwordHasher.HashPassword(request.Request.NewPassword);
                var newPasswordHash = new PasswordHash(hashedPassword, salt);

                // Update password and revoke all tokens
                user.ChangePassword(newPasswordHash);
                user.RevokeAllRefreshTokens();

                await _userRepository.UpdateAsync(user, cancellationToken);
                await _userRepository.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Password successfully changed for user: {UserId}", request.UserId);

                return Result<Unit>.Success(Unit.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while changing password for user: {UserId}", request.UserId);
                return Result<Unit>.Failure("An unexpected error occurred while changing password");
            }
        }
    }
}