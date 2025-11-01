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