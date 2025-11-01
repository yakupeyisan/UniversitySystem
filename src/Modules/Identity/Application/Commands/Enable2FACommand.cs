using Core.Domain.Repositories;
using Core.Domain.Results;
using Identity.Application.DTOs;
using Identity.Domain.Aggregates;
using Identity.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Commands;

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