using AutoMapper;
using Core.Domain.Results;
using Identity.Application.Abstractions;
using Identity.Application.DTOs;
using Identity.Domain.Aggregates;
using Identity.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Commands;

public class LoginCommand : IRequest<Result<LoginResponse>>
{
    public LoginRequest Request { get; set; }

    public LoginCommand(LoginRequest request)
    {
        Request = request ?? throw new ArgumentNullException(nameof(request));
    }

    public class Handler : IRequestHandler<LoginCommand, Result<LoginResponse>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<Handler> _logger;

        public Handler(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            ITokenService tokenService,
            IRefreshTokenRepository refreshTokenRepository,
            IMapper mapper,
            ILogger<Handler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _refreshTokenRepository = refreshTokenRepository ?? throw new ArgumentNullException(nameof(refreshTokenRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<LoginResponse>> Handle(
            LoginCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation(
                    "Login attempt for email: {Email}",
                    request.Request.Email);

                // Find user by email
                var user = await _userRepository.GetByEmailAsync(request.Request.Email, cancellationToken);
                if (user == null)
                {
                    _logger.LogWarning("Login failed - user not found for email: {Email}", request.Request.Email);
                    return Result<LoginResponse>.Failure("Invalid email or password");
                }

                // Check if user is deleted
                if (user.IsDeleted)
                {
                    _logger.LogWarning("Login failed - user account deleted: {Email}", request.Request.Email);
                    return Result<LoginResponse>.Failure("User account does not exist");
                }

                // Check if user account is locked
                if (user.IsLocked)
                {
                    _logger.LogWarning("Login failed - user account locked: {Email}", request.Request.Email);
                    return Result<LoginResponse>.Failure("Account is locked. Please try again later");
                }

                // Check if user is active
                if (!user.IsActive)
                {
                    _logger.LogWarning("Login failed - user account inactive: {Email}", request.Request.Email);
                    return Result<LoginResponse>.Failure("User account is not active");
                }

                // Verify password
                var passwordValid = _passwordHasher.VerifyPassword(
                    request.Request.Password,
                    user.PasswordHash.HashedPassword,
                    user.PasswordHash.Salt);

                if (!passwordValid)
                {
                    _logger.LogWarning("Login failed - invalid password for email: {Email}", request.Request.Email);
                    user.RecordFailedLoginAttempt();
                    await _userRepository.UpdateAsync(user, cancellationToken);
                    await _userRepository.SaveChangesAsync(cancellationToken);
                    return Result<LoginResponse>.Failure("Invalid email or password");
                }

                // Record successful login
                user.RecordSuccessfulLogin();
                await _userRepository.UpdateAsync(user, cancellationToken);

                // Generate tokens
                var accessToken = _tokenService.GenerateAccessToken(user);
                var refreshTokenString = _tokenService.GenerateRefreshToken();
                var refreshTokenEntity = RefreshToken.Create(
                    user.Id,
                    refreshTokenString,
                    DateTime.UtcNow.AddDays(7),
                    "", // IP Address - set from middleware
                    ""); // User Agent - set from middleware

                user.AddRefreshToken(refreshTokenEntity);
                await _userRepository.UpdateAsync(user, cancellationToken);
                await _userRepository.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("User successfully logged in: {Email}", request.Request.Email);

                var response = new LoginResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshTokenString,
                    ExpiresIn = _tokenService.AccessTokenExpirationSeconds,
                    User = _mapper.Map<UserDto>(user)
                };

                return Result<LoginResponse>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during login for email: {Email}", request.Request.Email);
                return Result<LoginResponse>.Failure("An unexpected error occurred during login");
            }
        }
    }
}