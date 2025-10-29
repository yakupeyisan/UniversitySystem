using Core.Domain.Repositories;
using Core.Domain.Results;
using Identity.Application.Abstractions;
using Identity.Domain.Aggregates;
using Identity.Domain.Specifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Commands;

public class ForgotPasswordCommand : IRequest<Result<string>>
{
    public ForgotPasswordCommand(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));

        Email = email.Trim().ToLower();
    }

    public string Email { get; set; } = string.Empty;

    public class Handler : IRequestHandler<ForgotPasswordCommand, Result<string>>
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<Handler> _logger;

        private readonly IRepository<User>
            _userRepository;

        public Handler(
            IRepository<User>
                userRepository,
            IEmailService emailService,
            ILogger<Handler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<string>> Handle(
            ForgotPasswordCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Forgot password request for email: {Email}", request.Email);

                var spec = new UserByEmailSpecification(request.Email);
                var user = await _userRepository.GetAsync(spec, cancellationToken);

                if (user == null)
                {
                    _logger.LogWarning("User not found for email: {Email}", request.Email);
                    // Return success anyway for security (don't reveal if email exists)
                    return Result<string>.Success("If email exists, password reset link has been sent");
                }

                var resetCode = GenerateResetCode();
                user.SetPasswordResetCode(resetCode);

                await _userRepository.UpdateAsync(user, cancellationToken);
                await _userRepository.SaveChangesAsync(cancellationToken);

                // Send email with reset code
                await _emailService.SendPasswordResetEmailAsync(user.Email.Value, resetCode, cancellationToken);

                _logger.LogInformation("Password reset code sent to email: {Email}", request.Email);
                return Result<string>.Success("Password reset link has been sent to your email");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing forgot password for email: {Email}", request.Email);
                return Result<string>.Failure("An unexpected error occurred");
            }
        }

        private static string GenerateResetCode()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
        }
    }
}