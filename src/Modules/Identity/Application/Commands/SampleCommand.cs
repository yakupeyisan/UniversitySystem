using AutoMapper;
using Core.Domain.Results;
using Identity.Application.Abstractions;
using Identity.Application.DTOs;
using Identity.Domain.Aggregates;
using Identity.Domain.Interfaces;
using Identity.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Commands;

public class ChangePasswordCommand : IRequest<Result<Unit>>
{
    public Guid UserId { get; set; }
    public ChangePasswordRequest Request { get; set; }

    public ChangePasswordCommand(Guid userId, ChangePasswordRequest request)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        UserId = userId;
        Request = request ?? throw new ArgumentNullException(nameof(request));
    }

    public class Handler : IRequestHandler<ChangePasswordCommand, Result<Unit>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger<Handler> _logger;

        public Handler(
            IUserRepository userRepository,
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
                    _logger.LogWarning("New password does not meet strength requirements for user: {UserId}", request.UserId);
                    return Result<Unit>.Failure($"New password does not meet requirements: {_passwordHasher.GetPasswordRequirements()}");
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

public class RegisterUserCommand : IRequest<Result<UserDto>>
{
    public RegisterUserRequest Request { get; set; }

    public RegisterUserCommand(RegisterUserRequest request)
    {
        Request = request ?? throw new ArgumentNullException(nameof(request));
    }

    public class Handler : IRequestHandler<RegisterUserCommand, Result<UserDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IMapper _mapper;
        private readonly ILogger<Handler> _logger;

        public Handler(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IMapper mapper,
            ILogger<Handler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<UserDto>> Handle(
            RegisterUserCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation(
                    "Attempting to register new user with email: {Email}",
                    request.Request.Email);

                // Check if user already exists
                if (await _userRepository.EmailExistsAsync(request.Request.Email, cancellationToken))
                {
                    _logger.LogWarning("Email already exists: {Email}", request.Request.Email);
                    return Result<UserDto>.Failure($"Email '{request.Request.Email}' is already registered");
                }

                // Validate password strength
                if (!_passwordHasher.ValidatePasswordStrength(request.Request.Password))
                {
                    _logger.LogWarning("Password does not meet strength requirements");
                    return Result<UserDto>.Failure($"Password does not meet requirements: {_passwordHasher.GetPasswordRequirements()}");
                }

                // Create email value object
                var email = new Email(request.Request.Email);

                // Hash password
                var (hashedPassword, salt) = _passwordHasher.HashPassword(request.Request.Password);
                var passwordHash = new PasswordHash(hashedPassword, salt);

                // Create user aggregate
                var user = User.Create(
                    email,
                    passwordHash,
                    request.Request.FirstName,
                    request.Request.LastName);

                // Save user
                await _userRepository.AddAsync(user, cancellationToken);
                await _userRepository.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("User successfully registered with email: {Email}", request.Request.Email);

                return Result<UserDto>.Success(_mapper.Map<UserDto>(user));
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Validation error during user registration");
                return Result<UserDto>.Failure(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during user registration");
                return Result<UserDto>.Failure("An unexpected error occurred during registration");
            }
        }
    }
}

#region Assign Role Command

public class AssignRoleCommand : IRequest<Result<UserDto>>
{
    public Guid UserId { get; set; }
    public AssignRoleRequest Request { get; set; }

    public AssignRoleCommand(Guid userId, AssignRoleRequest request)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        UserId = userId;
        Request = request ?? throw new ArgumentNullException(nameof(request));
    }

    public class Handler : IRequestHandler<AssignRoleCommand, Result<UserDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<Handler> _logger;

        public Handler(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IMapper mapper,
            ILogger<Handler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<UserDto>> Handle(
            AssignRoleCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation(
                    "Attempting to assign role {RoleId} to user {UserId}",
                    request.Request.RoleId,
                    request.UserId);

                // Get user
                var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
                if (user == null)
                {
                    _logger.LogWarning("User not found: {UserId}", request.UserId);
                    return Result<UserDto>.Failure("User not found");
                }

                // Get role
                var role = await _roleRepository.GetByIdAsync(request.Request.RoleId, cancellationToken);
                if (role == null)
                {
                    _logger.LogWarning("Role not found: {RoleId}", request.Request.RoleId);
                    return Result<UserDto>.Failure("Role not found");
                }

                // Add role to user
                user.AddRole(role);

                await _userRepository.UpdateAsync(user, cancellationToken);
                await _userRepository.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Role {RoleName} successfully assigned to user {UserId}",
                    role.RoleName,
                    request.UserId);

                return Result<UserDto>.Success(_mapper.Map<UserDto>(user));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while assigning role to user: {UserId}", request.UserId);
                return Result<UserDto>.Failure("An unexpected error occurred while assigning role");
            }
        }
    }
}

#endregion

#region Revoke Role Command

public class RevokeRoleCommand : IRequest<Result<UserDto>>
{
    public Guid UserId { get; set; }
    public RevokeRoleRequest Request { get; set; }

    public RevokeRoleCommand(Guid userId, RevokeRoleRequest request)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        UserId = userId;
        Request = request ?? throw new ArgumentNullException(nameof(request));
    }

    public class Handler : IRequestHandler<RevokeRoleCommand, Result<UserDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<Handler> _logger;

        public Handler(
            IUserRepository userRepository,
            IMapper mapper,
            ILogger<Handler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<UserDto>> Handle(
            RevokeRoleCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation(
                    "Attempting to revoke role {RoleId} from user {UserId}",
                    request.Request.RoleId,
                    request.UserId);

                // Get user
                var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
                if (user == null)
                {
                    _logger.LogWarning("User not found: {UserId}", request.UserId);
                    return Result<UserDto>.Failure("User not found");
                }

                // Remove role from user
                user.RemoveRole(request.Request.RoleId);

                await _userRepository.UpdateAsync(user, cancellationToken);
                await _userRepository.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Role successfully revoked from user: {UserId}", request.UserId);

                return Result<UserDto>.Success(_mapper.Map<UserDto>(user));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while revoking role from user: {UserId}", request.UserId);
                return Result<UserDto>.Failure("An unexpected error occurred while revoking role");
            }
        }
    }
}

#endregion

#region Grant Permission Command

public class GrantPermissionCommand : IRequest<Result<UserDto>>
{
    public Guid UserId { get; set; }
    public GrantPermissionRequest Request { get; set; }

    public GrantPermissionCommand(Guid userId, GrantPermissionRequest request)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        UserId = userId;
        Request = request ?? throw new ArgumentNullException(nameof(request));
    }

    public class Handler : IRequestHandler<GrantPermissionCommand, Result<UserDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<Handler> _logger;

        public Handler(
            IUserRepository userRepository,
            IPermissionRepository permissionRepository,
            IMapper mapper,
            ILogger<Handler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _permissionRepository = permissionRepository ?? throw new ArgumentNullException(nameof(permissionRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<UserDto>> Handle(
            GrantPermissionCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation(
                    "Attempting to grant permission {PermissionId} to user {UserId}",
                    request.Request.PermissionId,
                    request.UserId);

                // Get user
                var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
                if (user == null)
                {
                    _logger.LogWarning("User not found: {UserId}", request.UserId);
                    return Result<UserDto>.Failure("User not found");
                }

                // Get permission
                var permission = await _permissionRepository.GetByIdAsync(request.Request.PermissionId, cancellationToken);
                if (permission == null)
                {
                    _logger.LogWarning("Permission not found: {PermissionId}", request.Request.PermissionId);
                    return Result<UserDto>.Failure("Permission not found");
                }

                // Grant permission
                user.AddPermission(permission);

                await _userRepository.UpdateAsync(user, cancellationToken);
                await _userRepository.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Permission successfully granted to user: {UserId}", request.UserId);

                return Result<UserDto>.Success(_mapper.Map<UserDto>(user));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while granting permission to user: {UserId}", request.UserId);
                return Result<UserDto>.Failure("An unexpected error occurred while granting permission");
            }
        }
    }
}

#endregion

#region Revoke Permission Command

public class RevokePermissionCommand : IRequest<Result<UserDto>>
{
    public Guid UserId { get; set; }
    public RevokePermissionRequest Request { get; set; }

    public RevokePermissionCommand(Guid userId, RevokePermissionRequest request)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        UserId = userId;
        Request = request ?? throw new ArgumentNullException(nameof(request));
    }

    public class Handler : IRequestHandler<RevokePermissionCommand, Result<UserDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<Handler> _logger;

        public Handler(
            IUserRepository userRepository,
            IMapper mapper,
            ILogger<Handler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<UserDto>> Handle(
            RevokePermissionCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation(
                    "Attempting to revoke permission {PermissionId} from user {UserId}",
                    request.Request.PermissionId,
                    request.UserId);

                // Get user
                var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
                if (user == null)
                {
                    _logger.LogWarning("User not found: {UserId}", request.UserId);
                    return Result<UserDto>.Failure("User not found");
                }

                // Revoke permission
                user.RemovePermission(request.Request.PermissionId);

                await _userRepository.UpdateAsync(user, cancellationToken);
                await _userRepository.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Permission successfully revoked from user: {UserId}", request.UserId);

                return Result<UserDto>.Success(_mapper.Map<UserDto>(user));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while revoking permission from user: {UserId}", request.UserId);
                return Result<UserDto>.Failure("An unexpected error occurred while revoking permission");
            }
        }
    }
}

#endregion


public class UpdateUserCommand : IRequest<Result<UserDto>>
{
    public Guid UserId { get; set; }
    public UpdateUserRequest Request { get; set; }

    public UpdateUserCommand(Guid userId, UpdateUserRequest request)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        UserId = userId;
        Request = request ?? throw new ArgumentNullException(nameof(request));
    }

    public class Handler : IRequestHandler<UpdateUserCommand, Result<UserDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<Handler> _logger;

        public Handler(
            IUserRepository userRepository,
            IMapper mapper,
            ILogger<Handler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<UserDto>> Handle(
            UpdateUserCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Attempting to update user: {UserId}", request.UserId);

                // Get user
                var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
                if (user == null)
                {
                    _logger.LogWarning("User not found: {UserId}", request.UserId);
                    return Result<UserDto>.Failure("User not found");
                }

                // Update profile
                user.UpdateProfile(request.Request.FirstName, request.Request.LastName);

                await _userRepository.UpdateAsync(user, cancellationToken);
                await _userRepository.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("User successfully updated: {UserId}", request.UserId);

                return Result<UserDto>.Success(_mapper.Map<UserDto>(user));
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Validation error while updating user: {UserId}", request.UserId);
                return Result<UserDto>.Failure(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while updating user: {UserId}", request.UserId);
                return Result<UserDto>.Failure("An unexpected error occurred while updating user");
            }
        }
    }
}







