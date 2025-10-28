using Core.Domain.Pagination;
using Core.Domain.Results;
using Identity.Application.Commands;
using Identity.Application.DTOs;
using Identity.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// User management endpoints - CRUD, Search, Role/Permission assignment
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IMediator mediator, ILogger<UsersController> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Get user by ID
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>User details with roles and permissions</returns>
    [HttpGet("{userId}")]
    [ProducesResponseType(typeof(Result<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUser(Guid userId)
    {
        _logger.LogInformation("Getting user: {UserId}", userId);

        try
        {
            var query = new GetUserByIdQuery(userId);
            var result = await _mediator.Send(query);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("User not found: {UserId}", userId);
                return NotFound(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user: {UserId}", userId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error retrieving user" });
        }
    }

    /// <summary>
    /// Get all active users
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10)</param>
    /// <returns>Paginated list of users</returns>
    [HttpGet]
    [ProducesResponseType(typeof(Result<PagedList<UserDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAllUsers(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        _logger.LogInformation("Getting all users - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);

        try
        {
            var pagedRequest = new PagedRequest { PageNumber = pageNumber, PageSize = pageSize };
            var query = new ListUsersQuery(pagedRequest);
            var result = await _mediator.Send(query);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting users");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error retrieving users" });
        }
    }

    /// <summary>
    /// Search users by name or email
    /// </summary>
    /// <param name="searchTerm">Search term (name or email)</param>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10)</param>
    /// <returns>Paginated search results</returns>
    [HttpGet("search")]
    [ProducesResponseType(typeof(Result<PagedList<UserDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SearchUsers(
        [FromQuery] string searchTerm,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        _logger.LogInformation("Searching users - Term: {SearchTerm}, Page: {PageNumber}", searchTerm, pageNumber);

        try
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return BadRequest(new { message = "Search term is required" });

            var pagedRequest = new PagedRequest { PageNumber = pageNumber, PageSize = pageSize };
            var query = new SearchUsersQuery(searchTerm, pageNumber, pageSize);
            var result = await _mediator.Send(query);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching users");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error searching users" });
        }
    }

    /// <summary>
    /// Get user by email
    /// </summary>
    /// <param name="email">User email</param>
    /// <returns>User details</returns>
    [HttpGet("by-email")]
    [ProducesResponseType(typeof(Result<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetByEmail([FromQuery] string email)
    {
        _logger.LogInformation("Getting user by email: {Email}", email);

        try
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest(new { message = "Email is required" });

            var query = new GetUserByEmailQuery(email);
            var result = await _mediator.Send(query);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("User not found by email: {Email}", email);
                return NotFound(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by email");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error retrieving user" });
        }
    }

    /// <summary>
    /// Update user profile
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="request">Updated user details</param>
    /// <returns>Updated user details</returns>
    [HttpPut("{userId}")]
    [ProducesResponseType(typeof(Result<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] UpdateUserRequest request)
    {
        _logger.LogInformation("Updating user: {UserId}", userId);

        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var command = new UpdateUserCommand(userId, request);
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("User update failed: {UserId}", userId);
                return BadRequest(result);
            }

            _logger.LogInformation("User successfully updated: {UserId}", userId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user: {UserId}", userId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error updating user" });
        }
    }

    /// <summary>
    /// Delete user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Success message</returns>
    [HttpDelete("{userId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteUser(Guid userId)
    {
        _logger.LogInformation("Deleting user: {UserId}", userId);

        try
        {
            var command = new DeleteUserCommand(userId);
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("User deletion failed: {UserId}", userId);
                return NotFound(result);
            }

            _logger.LogInformation("User successfully deleted: {UserId}", userId);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user: {UserId}", userId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error deleting user" });
        }
    }

    /// <summary>
    /// Get user's roles
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>List of user's roles</returns>
    [HttpGet("{userId}/roles")]
    [ProducesResponseType(typeof(Result<List<RoleDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUserRoles(Guid userId)
    {
        _logger.LogInformation("Getting roles for user: {UserId}", userId);

        try
        {
            var query = new GetUserRolesQuery(userId);
            var result = await _mediator.Send(query);

            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user roles: {UserId}", userId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error retrieving user roles" });
        }
    }

    /// <summary>
    /// Get user's permissions
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>List of user's permissions</returns>
    [HttpGet("{userId}/permissions")]
    [ProducesResponseType(typeof(Result<List<PermissionDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUserPermissions(Guid userId)
    {
        _logger.LogInformation("Getting permissions for user: {UserId}", userId);

        try
        {
            var query = new GetUserPermissionsQuery(userId);
            var result = await _mediator.Send(query);

            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user permissions: {UserId}", userId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error retrieving user permissions" });
        }
    }

    /// <summary>
    /// Assign role to user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="request">Role assignment request</param>
    /// <returns>Updated user details</returns>
    [HttpPost("{userId}/roles")]
    [ProducesResponseType(typeof(Result<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AssignRole(Guid userId, [FromBody] AssignRoleRequest request)
    {
        _logger.LogInformation("Assigning role {RoleId} to user {UserId}", request.RoleId, userId);

        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var command = new AssignRoleCommand(userId, request);
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Role assignment failed for user: {UserId}", userId);
                return BadRequest(result);
            }

            _logger.LogInformation("Role successfully assigned to user: {UserId}", userId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning role to user: {UserId}", userId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error assigning role" });
        }
    }

    /// <summary>
    /// Remove role from user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="request">Role revocation request</param>
    /// <returns>Updated user details</returns>
    [HttpDelete("{userId}/roles")]
    [ProducesResponseType(typeof(Result<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RemoveRole(Guid userId, [FromBody] RevokeRoleRequest request)
    {
        _logger.LogInformation("Removing role {RoleId} from user {UserId}", request.RoleId, userId);

        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var command = new RevokeRoleCommand(userId, request);
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Role removal failed for user: {UserId}", userId);
                return BadRequest(result);
            }

            _logger.LogInformation("Role successfully removed from user: {UserId}", userId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing role from user: {UserId}", userId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error removing role" });
        }
    }

    /// <summary>
    /// Grant permission to user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="request">Permission grant request</param>
    /// <returns>Updated user details</returns>
    [HttpPost("{userId}/permissions")]
    [ProducesResponseType(typeof(Result<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GrantPermission(Guid userId, [FromBody] GrantPermissionRequest request)
    {
        _logger.LogInformation("Granting permission {PermissionId} to user {UserId}", request.PermissionId, userId);

        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var command = new GrantPermissionCommand(userId, request);
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Permission grant failed for user: {UserId}", userId);
                return BadRequest(result);
            }

            _logger.LogInformation("Permission successfully granted to user: {UserId}", userId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error granting permission to user: {UserId}", userId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error granting permission" });
        }
    }

    /// <summary>
    /// Revoke permission from user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="request">Permission revocation request</param>
    /// <returns>Updated user details</returns>
    [HttpDelete("{userId}/permissions")]
    [ProducesResponseType(typeof(Result<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RevokePermission(Guid userId, [FromBody] RevokePermissionRequest request)
    {
        _logger.LogInformation("Revoking permission {PermissionId} from user {UserId}", request.PermissionId, userId);

        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var command = new RevokePermissionCommand(userId, request);
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Permission revocation failed for user: {UserId}", userId);
                return BadRequest(result);
            }

            _logger.LogInformation("Permission successfully revoked from user: {UserId}", userId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking permission from user: {UserId}", userId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error revoking permission" });
        }
    }

    /// <summary>
    /// Lock user account
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Success message</returns>
    [HttpPost("{userId}/lock")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> LockUser(Guid userId)
    {
        _logger.LogInformation("Locking user account: {UserId}", userId);

        try
        {
            var command = new LockUserCommand(userId);
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(new { success = true, message = "User account locked successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error locking user: {UserId}", userId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error locking user account" });
        }
    }

    /// <summary>
    /// Unlock user account
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Success message</returns>
    [HttpPost("{userId}/unlock")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UnlockUser(Guid userId)
    {
        _logger.LogInformation("Unlocking user account: {UserId}", userId);

        try
        {
            var command = new UnlockUserCommand(userId);
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(new { success = true, message = "User account unlocked successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unlocking user: {UserId}", userId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error unlocking user account" });
        }
    }
}