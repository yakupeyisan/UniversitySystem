using Core.Domain.Repositories;
using Core.Domain.Results;
using Identity.Domain.Aggregates;
using Identity.Domain.Specifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Commands;

/// <summary>
/// Backup code doðrula ve kullanýldýðýný iþaretle
/// </summary>
public class ValidateBackupCodeCommand : IRequest<Result<bool>>
{
    public ValidateBackupCodeCommand(Guid userId, string backupCode)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        if (string.IsNullOrWhiteSpace(backupCode))
            throw new ArgumentException("Backup code cannot be empty", nameof(backupCode));

        UserId = userId;
        BackupCode = backupCode.Trim().ToUpperInvariant();
    }

    public Guid UserId { get; set; }
    public string BackupCode { get; set; }

    public class Handler : IRequestHandler<ValidateBackupCodeCommand, Result<bool>>
    {
        private readonly ILogger<Handler> _logger;
        private readonly IRepository<TwoFactorToken> _twoFactorRepository;

        public Handler(
            IRepository<TwoFactorToken> twoFactorRepository,
            ILogger<Handler> logger)
        {
            _twoFactorRepository = twoFactorRepository ?? throw new ArgumentNullException(nameof(twoFactorRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<bool>> Handle(
            ValidateBackupCodeCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Validating backup code for user {UserId}", request.UserId);

                // Aktif 2FA token'ý al
                var spec = new ActiveTwoFactorTokensSpecification(request.UserId);

                var tokens = await _twoFactorRepository.GetAllAsync(spec, cancellationToken);
                var token = tokens.FirstOrDefault();

                if (token == null)
                {
                    _logger.LogWarning("No active 2FA token found for user {UserId}", request.UserId);
                    return Result<bool>.Failure("2FA not enabled");
                }

                // Backup code'ý kontrol et
                var backupCodes = token.GetBackupCodes();
                if (!backupCodes.Contains(request.BackupCode))
                {
                    _logger.LogWarning("Invalid backup code for user {UserId}", request.UserId);
                    return Result<bool>.Failure("Invalid backup code");
                }

                // Backup code'u kullan ve güncelle
                token.UseBackupCode(request.BackupCode);
                await _twoFactorRepository.UpdateAsync(token, cancellationToken);
                await _twoFactorRepository.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Backup code validated and used for user {UserId}", request.UserId);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating backup code for user {UserId}", request.UserId);
                return Result<bool>.Failure("An error occurred while validating backup code");
            }
        }
    }
}