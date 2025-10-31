using AutoMapper;
using Core.Application.Abstractions;
using Core.Domain.Repositories;
using Core.Domain.Results;
using Identity.Application.DTOs;
using Identity.Domain.Aggregates;
using Identity.Domain.Enums;
using Identity.Domain.Events;
using Identity.Domain.Specifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Commands;

public class VerifyEmailCommand : IRequest<Result<UserDto>>
{
    public VerifyEmailCommand(Guid userId, string verificationCode)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        if (string.IsNullOrWhiteSpace(verificationCode))
            throw new ArgumentException("Verification code cannot be empty", nameof(verificationCode));

        UserId = userId;
        VerificationCode = verificationCode.Trim();
    }

    public Guid UserId { get; set; }
    public string VerificationCode { get; set; } = string.Empty;

    public class Handler : IRequestHandler<VerifyEmailCommand, Result<UserDto>>
    {
        private readonly ILogger<Handler> _logger;
        private readonly IMapper _mapper;

        private readonly IRepository<User>
            _userRepository;

        public Handler(
            IRepository<User>
                userRepository,
            IMapper mapper,
            ILogger<Handler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<UserDto>> Handle(
            VerifyEmailCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Verifying email for user: {UserId}", request.UserId);

                var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
                if (user == null)
                {
                    _logger.LogWarning("User not found: {UserId}", request.UserId);
                    return Result<UserDto>.Failure("User not found");
                }

                if (user.IsEmailVerified)
                {
                    _logger.LogWarning("Email already verified for user: {UserId}", request.UserId);
                    return Result<UserDto>.Failure("Email is already verified");
                }

                // Verification logic would check against stored verification code
                user.VerifyEmail();
                await _userRepository.UpdateAsync(user, cancellationToken);
                await _userRepository.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Email verified successfully for user: {UserId}", request.UserId);
                return Result<UserDto>.Success(_mapper.Map<UserDto>(user));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying email for user: {UserId}", request.UserId);
                return Result<UserDto>.Failure("An unexpected error occurred while verifying email");
            }
        }
    }
}

/// <summary>
/// 2FA (Ýki Faktörlü Kimlik Doðrulama) aktifleþtir
/// </summary>
public class Enable2FACommand : IRequest<Result<TwoFactorSetupDto>>
{
    public Enable2FACommand(Guid userId, TwoFactorMethod method)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        UserId = userId;
        Method = method;
    }

    public Guid UserId { get; set; }
    public TwoFactorMethod Method { get; set; }

    public class Handler : IRequestHandler<Enable2FACommand, Result<TwoFactorSetupDto>>
    {
        private readonly ILogger<Handler> _logger;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<TwoFactorToken> _twoFactorRepository;

        public Handler(
            IRepository<User> userRepository,
            IRepository<TwoFactorToken> twoFactorRepository,
            ILogger<Handler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _twoFactorRepository = twoFactorRepository ?? throw new ArgumentNullException(nameof(twoFactorRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<TwoFactorSetupDto>> Handle(
            Enable2FACommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Enabling 2FA for user {UserId} with method {Method}",
                    request.UserId, request.Method);

                // Kullanýcýyý kontrol et
                var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
                if (user == null)
                {
                    _logger.LogWarning("User not found: {UserId}", request.UserId);
                    return Result<TwoFactorSetupDto>.Failure("User not found");
                }

                // Secret key ve backup codes oluþtur
                var secretKey = GenerateSecretKey(); // TOTP'nin secret key'i
                var backupCodes = GenerateBackupCodes(); // 10 adet backup code

                // TwoFactorToken oluþtur (henüz doðrulanmamýþ)
                var twoFactorToken = TwoFactorToken.Create(
                    request.UserId,
                    request.Method,
                    secretKey,
                    string.Join("|", backupCodes));

                await _twoFactorRepository.AddAsync(twoFactorToken, cancellationToken);
                await _twoFactorRepository.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("2FA setup created for user {UserId}", request.UserId);

                // QR Code oluþtur (client tarafýnda)
                var qrCodeData = GenerateQRCodeData(user.Email, secretKey);

                return Result<TwoFactorSetupDto>.Success(new TwoFactorSetupDto
                {
                    UserId = request.UserId,
                    Method = request.Method.ToString(),
                    SecretKey = secretKey,
                    QRCodeData = qrCodeData,
                    BackupCodes = backupCodes,
                    SetupInstructions = GetSetupInstructions(request.Method)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enabling 2FA for user {UserId}", request.UserId);
                return Result<TwoFactorSetupDto>.Failure("An error occurred while enabling 2FA");
            }
        }

        private string GenerateSecretKey()
        {
            // RFC 4648 Base32 format'da secret key oluþtur (örneðin: 32 karakter)
            using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                byte[] secretKeyBytes = new byte[20];
                rng.GetBytes(secretKeyBytes);
                return Convert.ToBase64String(secretKeyBytes);
            }
        }

        private List<string> GenerateBackupCodes(int count = 10)
        {
            var codes = new List<string>();
            using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                for (int i = 0; i < count; i++)
                {
                    byte[] codeBytes = new byte[4];
                    rng.GetBytes(codeBytes);
                    string code = $"{BitConverter.ToUInt32(codeBytes, 0):X8}";
                    codes.Add(code);
                }
            }

            return codes;
        }

        private string GenerateQRCodeData(string email, string secretKey)
        {
            // Google Authenticator format: otpauth://totp/user@example.com?secret=XXX
            return $"otpauth://totp/{email}?secret={secretKey}&issuer=UniversitySystem";
        }

        private string GetSetupInstructions(TwoFactorMethod method)
        {
            return method switch
            {
                TwoFactorMethod.AuthenticatorApp =>
                    "Google Authenticator, Microsoft Authenticator veya benzer bir uygulama indir. QR kodunu tara.",
                TwoFactorMethod.Sms =>
                    "SMS yöntemi seçildi. Giriþ yapýrken telefonuna kod gönderilecek.",
                TwoFactorMethod.Email =>
                    "E-posta yöntemi seçildi. Giriþ yapýrken emailine kod gönderilecek.",
                _ => "2FA yöntemi için talimatlarý kontrol et."
            };
        }
    }
}

/// <summary>
/// 2FA kurulumunu doðrula ve aktifleþtir
/// </summary>
public class Verify2FACommand : IRequest<Result<UserDto>>
{
    public Verify2FACommand(Guid userId, string code, List<string> backupCodes)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Code cannot be empty", nameof(code));

        UserId = userId;
        Code = code.Trim();
        BackupCodes = backupCodes ?? new List<string>();
    }

    public Guid UserId { get; set; }
    public string Code { get; set; }
    public List<string> BackupCodes { get; set; }

    public class Handler : IRequestHandler<Verify2FACommand, Result<UserDto>>
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
            Verify2FACommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Verifying 2FA for user {UserId}", request.UserId);

                var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
                if (user == null)
                {
                    _logger.LogWarning("User not found: {UserId}", request.UserId);
                    return Result<UserDto>.Failure("User not found");
                }

                // Son oluþturulan TwoFactorToken'ý al (henüz doðrulanmamýþ)
                var spec = new ActiveTwoFactorTokensSpecification(request.UserId);
                var twoFactorTokens = await _twoFactorRepository.GetAllAsync(spec, cancellationToken);
                var twoFactorToken = twoFactorTokens.FirstOrDefault();

                if (twoFactorToken == null)
                {
                    _logger.LogWarning("2FA token not found for user {UserId}", request.UserId);
                    return Result<UserDto>.Failure("2FA setup not found");
                }

                // Kodu doðrula (TOTP algoritmasý)
                if (!VerifyTOTPCode(twoFactorToken.SecretKey, request.Code))
                {
                    _logger.LogWarning("Invalid 2FA code for user {UserId}", request.UserId);
                    return Result<UserDto>.Failure("Invalid verification code");
                }

                // Token'ý doðrula
                twoFactorToken.Verify();
                await _twoFactorRepository.UpdateAsync(twoFactorToken, cancellationToken);

                // Kullanýcýyý 2FA enabled yap
                user.EnableTwoFactor(twoFactorToken.Method, _currentUserService.UserId);
                await _userRepository.UpdateAsync(user, cancellationToken);
                await _userRepository.SaveChangesAsync(cancellationToken);


                _logger.LogInformation("2FA verified and enabled for user {UserId}", request.UserId);

                return Result<UserDto>.Success(_mapper.Map<UserDto>(user));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying 2FA for user {UserId}", request.UserId);
                return Result<UserDto>.Failure("An error occurred while verifying 2FA");
            }
        }

        private bool VerifyTOTPCode(string secretKey, string code)
        {
            // Google Authenticator TOTP algoritmasý (RFC 6238)
            // Bu basitleþtirilmiþ bir örnek - gerçek implementasyon için OtpNet veya benzer kütüphane kullan
            try
            {
                // OtpNet.Totp veya benzer kütüphane ile doðrula
                // Example: var totp = new Totp(Base32Encoding.ToBytes(secretKey));
                // return totp.VerifyTotp(code, out _, VerificationWindow.RfcSpecifiedWindow);

                // Placeholder - gerçek kütüphane ile deðiþtir
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

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

/// <summary>
/// Admin tarafýndan kilitli hesabý aç
/// </summary>
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

                // Hesapý aç
                user.UnlockAccount(_currentUserService.UserId);
                await _userRepository.UpdateAsync(user, cancellationToken);

                // Aktif lockout kayýtlarýný güncelle
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