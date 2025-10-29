using Core.Domain.Repositories;
using Core.Domain.Results;
using Identity.Application.Abstractions;
using Identity.Application.DTOs;
using Identity.Domain.Aggregates;
using Identity.Domain.Specifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Commands;

public class ResetPasswordCommand : IRequest<Result<bool>>
{
    public ResetPasswordCommand(ResetPasswordRequest request)
    {
        Request = request;
    }

    public ResetPasswordRequest Request { get; set; }

    public class Handler : IRequestHandler<ResetPasswordCommand, Result<bool>>
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

        public async Task<Result<bool>> Handle(
            ResetPasswordCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Resetting password for email: {Email}", request.Request.Email);

                var spec = new UserByEmailSpecification(request.Request.Email);
                var user = await _userRepository.GetAsync(spec, cancellationToken);

                if (user == null || !user.ValidatePasswordResetCode(request.Request.ResetCode))
                {
                    _logger.LogWarning("Invalid reset code for email: {Email}", request.Request.Email);
                    return Result<bool>.Failure("Invalid or expired reset code");
                }

                var password = _passwordHasher.HashPassword(request.Request.NewPassword);
                user.ResetPassword(password.HashedPassword, password.Salt);

                await _userRepository.UpdateAsync(user, cancellationToken);
                await _userRepository.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Password reset successfully for email: {Email}", request.Request.Email);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password for email: {Email}", request.Request.Email);
                return Result<bool>.Failure("An unexpected error occurred while resetting password");
            }
        }
    }
}