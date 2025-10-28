using Core.Domain.Results;
using Identity.Application.Commands;
using Identity.Application.DTOs;
using Identity.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Permission management endpoints - CRUD and Listing
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
[Authorize]
public class PermissionsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<PermissionsController> _logger;

    public PermissionsController(IMediator mediator, ILogger<PermissionsController> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Get permission by ID
    /// </summary>
    /// <param name="permissionId">Permission ID</param>
    /// <returns>Permission details</returns>
    [HttpGet("{permissionId}")]
    [ProducesResponseType(typeof(Result<PermissionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetPermission(Guid permissionId)
    {
        _logger.LogInformation("Getting permission: {PermissionId}", permissionId);

        try
        {
            var query = new GetPermissionByIdQuery(permissionId);
            var result = await _mediator.Send(query);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Permission not found: {PermissionId}", permissionId);
                return NotFound(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting permission: {PermissionId}", permissionId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error retrieving permission" });
        }
    }

    /// <summary>
    /// Get all active permissions
    /// </summary>
    /// <returns>List of all active permissions</returns>
    [HttpGet]
    [ProducesResponseType(typeof(Result<List<PermissionDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [AllowAnonymous] // Optionally allow anonymous access to list permissions
    public async Task<IActionResult> GetAllPermissions()
    {
        _logger.LogInformation("Getting all permissions");

        try
        {
            var query = new ListPermissionsQuery();
            var result = await _mediator.Send(query);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting permissions");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error retrieving permissions" });
        }
    }

    /// <summary>
    /// Get permission by name
    /// </summary>
    /// <param name="name">Permission name</param>
    /// <returns>Permission details</returns>
    [HttpGet("by-name/{name}")]
    [ProducesResponseType(typeof(Result<PermissionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [AllowAnonymous]
    public async Task<IActionResult> GetByName(string name)
    {
        _logger.LogInformation("Getting permission by name: {PermissionName}", name);

        try
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest(new { message = "Permission name is required" });

            var query = new GetPermissionByNameQuery(name);
            var result = await _mediator.Send(query);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Permission not found by name: {PermissionName}", name);
                return NotFound(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting permission by name");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error retrieving permission" });
        }
    }

    /// <summary>
    /// Search permissions by name or description
    /// </summary>
    /// <param name="searchTerm">Search term</param>
    /// <returns>List of matching permissions</returns>
    [HttpGet("search")]
    [ProducesResponseType(typeof(Result<List<PermissionDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [AllowAnonymous]
    public async Task<IActionResult> SearchPermissions([FromQuery] string searchTerm)
    {
        _logger.LogInformation("Searching permissions - Term: {SearchTerm}", searchTerm);

        try
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return BadRequest(new { message = "Search term is required" });

            var query = new SearchPermissionsQuery(searchTerm);
            var result = await _mediator.Send(query);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching permissions");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error searching permissions" });
        }
    }

    /// <summary>
    /// Get permissions by type
    /// </summary>
    /// <param name="type">Permission type</param>
    /// <returns>List of permissions of the specified type</returns>
    [HttpGet("by-type/{type}")]
    [ProducesResponseType(typeof(Result<List<PermissionDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [AllowAnonymous]
    public async Task<IActionResult> GetByType(int type)
    {
        _logger.LogInformation("Getting permissions by type: {Type}", type);

        try
        {
            if (!Enum.IsDefined(typeof(Identity.Domain.Enums.PermissionType), type))
                return BadRequest(new { message = "Invalid permission type" });

            var query = new GetPermissionsByTypeQuery((Identity.Domain.Enums.PermissionType)type);
            var result = await _mediator.Send(query);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting permissions by type");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error retrieving permissions" });
        }
    }

    /// <summary>
    /// Create new permission
    /// </summary>
    /// <param name="request">Permission creation request</param>
    /// <returns>Created permission details</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Result<PermissionDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreatePermission([FromBody] CreatePermissionRequest request)
    {
        _logger.LogInformation("Creating new permission: {PermissionName}", request.PermissionName);

        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var command = new CreatePermissionCommand(request);
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Permission creation failed: {PermissionName}", request.PermissionName);
                return BadRequest(result);
            }

            _logger.LogInformation("Permission successfully created: {PermissionName}", request.PermissionName);
            return CreatedAtAction(nameof(GetPermission), new { permissionId = result.Value?.Id }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating permission");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error creating permission" });
        }
    }

    /// <summary>
    /// Update permission
    /// </summary>
    /// <param name="permissionId">Permission ID</param>
    /// <param name="request">Permission update request</param>
    /// <returns>Updated permission details</returns>
    [HttpPut("{permissionId}")]
    [ProducesResponseType(typeof(Result<PermissionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdatePermission(Guid permissionId, [FromBody] UpdatePermissionRequest request)
    {
        _logger.LogInformation("Updating permission: {PermissionId}", permissionId);

        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var command = new UpdatePermissionCommand(permissionId, request);
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Permission update failed: {PermissionId}", permissionId);
                return BadRequest(result);
            }

            _logger.LogInformation("Permission successfully updated: {PermissionId}", permissionId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating permission: {PermissionId}", permissionId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error updating permission" });
        }
    }

    /// <summary>
    /// Delete permission
    /// </summary>
    /// <param name="permissionId">Permission ID</param>
    /// <returns>Success message</returns>
    [HttpDelete("{permissionId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeletePermission(Guid permissionId)
    {
        _logger.LogInformation("Deleting permission: {PermissionId}", permissionId);

        try
        {
            var command = new DeletePermissionCommand(permissionId);
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Permission deletion failed: {PermissionId}", permissionId);
                return NotFound(result);
            }

            _logger.LogInformation("Permission successfully deleted: {PermissionId}", permissionId);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting permission: {PermissionId}", permissionId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error deleting permission" });
        }
    }

    /// <summary>
    /// Activate permission
    /// </summary>
    /// <param name="permissionId">Permission ID</param>
    /// <returns>Success message</returns>
    [HttpPost("{permissionId}/activate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ActivatePermission(Guid permissionId)
    {
        _logger.LogInformation("Activating permission: {PermissionId}", permissionId);

        try
        {
            var command = new ActivatePermissionCommand(permissionId);
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(new { success = true, message = "Permission activated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error activating permission: {PermissionId}", permissionId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error activating permission" });
        }
    }

    /// <summary>
    /// Deactivate permission
    /// </summary>
    /// <param name="permissionId">Permission ID</param>
    /// <returns>Success message</returns>
    [HttpPost("{permissionId}/deactivate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeactivatePermission(Guid permissionId)
    {
        _logger.LogInformation("Deactivating permission: {PermissionId}", permissionId);

        try
        {
            var command = new DeactivatePermissionCommand(permissionId);
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(new { success = true, message = "Permission deactivated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deactivating permission: {PermissionId}", permissionId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error deactivating permission" });
        }
    }
}