using Core.Domain.Results;
using Identity.Application.Commands;
using Identity.Application.DTOs;
using Identity.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
///     Authentication endpoints - Login, Register, Token Management
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator, ILogger<AuthController> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    ///     Register new user
    /// </summary>
    /// <param name="request">Registration details</param>
    /// <returns>Newly created user with tokens</returns>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(Result<UserDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result<UserDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
    {
        _logger.LogInformation("User registration attempt for email: {Email}", request.Email);

        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors) });

            var command = new RegisterUserCommand(request);
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Registration failed for email: {Email}", request.Email);
                return BadRequest(result);
            }

            _logger.LogInformation("User successfully registered: {Email}", request.Email);
            return CreatedAtAction(nameof(Register), result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user registration for email: {Email}", request.Email);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Registration failed" });
        }
    }

    /// <summary>
    ///     Login user with email and password
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <returns>Access token and refresh token</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(Result<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<LoginResponse>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        _logger.LogInformation("Login attempt for email: {Email}", request.Email);

        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var command = new LoginCommand(request);
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Login failed for email: {Email}", request.Email);
                return Unauthorized(result);
            }

            _logger.LogInformation("User successfully logged in: {Email}", request.Email);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for email: {Email}", request.Email);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Login failed" });
        }
    }

    /// <summary>
    ///     Refresh access token using refresh token
    /// </summary>
    /// <param name="request">Refresh token request</param>
    /// <returns>New access token</returns>
    [HttpPost("refresh-token")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(Result<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<LoginResponse>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        _logger.LogInformation("Refresh token attempt");

        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var command = new RefreshTokenCommand(request.RefreshToken);
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Refresh token failed");
                return Unauthorized(result);
            }

            _logger.LogInformation("Token successfully refreshed");
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token refresh");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Token refresh failed" });
        }
    }

    /// <summary>
    ///     Change user password
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="request">Password change details</param>
    /// <returns>Success message</returns>
    [HttpPost("{userId}/change-password")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ChangePassword(Guid userId, [FromBody] ChangePasswordRequest request)
    {
        _logger.LogInformation("Password change attempt for user: {UserId}", userId);

        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var command = new ChangePasswordCommand(userId, request);
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Password change failed for user: {UserId}", userId);
                return BadRequest(result);
            }

            _logger.LogInformation("Password successfully changed for user: {UserId}", userId);
            return Ok(new { success = true, message = "Password changed successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during password change for user: {UserId}", userId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Password change failed" });
        }
    }

    /// <summary>
    ///     Logout user and revoke refresh tokens
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Success message</returns>
    [HttpPost("{userId}/logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout(Guid userId)
    {
        _logger.LogInformation("Logout attempt for user: {UserId}", userId);

        try
        {
            var command = new LogoutCommand(userId);
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Logout failed for user: {UserId}", userId);
                return BadRequest(result);
            }

            _logger.LogInformation("User successfully logged out: {UserId}", userId);
            return Ok(new { success = true, message = "Logged out successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout for user: {UserId}", userId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Logout failed" });
        }
    }

    /// <summary>
    ///     Verify email address
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="request">Email verification token</param>
    /// <returns>Success message</returns>
    [HttpPost("{userId}/verify-email")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> VerifyEmail(Guid userId, [FromBody] VerifyEmailRequest request)
    {
        _logger.LogInformation("Email verification attempt for user: {UserId}", userId);

        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var command = new VerifyEmailCommand(userId, request.VerificationCode);
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Email verification failed for user: {UserId}", userId);
                return BadRequest(result);
            }

            _logger.LogInformation("Email successfully verified for user: {UserId}", userId);
            return Ok(new { success = true, message = "Email verified successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during email verification for user: {UserId}", userId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Email verification failed" });
        }
    }

    /// <summary>
    ///     Request password reset
    /// </summary>
    /// <param name="request">Email address</param>
    /// <returns>Success message</returns>
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        _logger.LogInformation("Password reset request for email: {Email}", request.Email);

        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var command = new ForgotPasswordCommand(request.Email);
            var result = await _mediator.Send(command);

            // Don't reveal if email exists or not (security best practice)
            _logger.LogInformation("Password reset request processed for email: {Email}", request.Email);
            return Ok(new { success = true, message = "If email exists, password reset link has been sent" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during password reset request for email: {Email}", request.Email);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Password reset request failed" });
        }
    }

    /// <summary>
    ///     Reset password with token
    /// </summary>
    /// <param name="request">Reset token and new password</param>
    /// <returns>Success message</returns>
    [HttpPost("reset-password")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        _logger.LogInformation("Password reset attempt");

        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var command = new ResetPasswordCommand(request);
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Password reset failed");
                return BadRequest(result);
            }

            _logger.LogInformation("Password successfully reset");
            return Ok(new { success = true, message = "Password has been reset successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during password reset");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Password reset failed" });
        }
    }

    /// <summary>
    ///     Get current user profile
    /// </summary>
    /// <returns>Current user details</returns>
    [HttpGet("profile")]
    [Authorize]
    [ProducesResponseType(typeof(Result<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetProfile()
    {
        _logger.LogInformation("Getting current user profile");

        try
        {
            // Extract user ID from JWT claims
            var userIdClaim = User.FindFirst("sub")?.Value ??
                              User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")
                                  ?.Value;

            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                _logger.LogWarning("Invalid user ID in token");
                return Unauthorized();
            }

            var query = new GetUserByIdQuery(userId);
            var result = await _mediator.Send(query);

            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user profile");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Failed to get profile" });
        }
    }
}