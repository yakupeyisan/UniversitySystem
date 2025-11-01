using Core.Domain.Pagination;
using Core.Domain.Results;
using Identity.Application.Commands;
using Identity.Application.DTOs;
using Identity.Application.Queries;
using Identity.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace API.Controllers;
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
[Authorize]
public class RolesController : ControllerBase
{
    private readonly ILogger<RolesController> _logger;
    private readonly IMediator _mediator;
    public RolesController(IMediator mediator, ILogger<RolesController> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    [HttpGet("{roleId}")]
    [ProducesResponseType(typeof(Result<RoleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetRole(Guid roleId)
    {
        _logger.LogInformation("Getting role: {RoleId}", roleId);
        try
        {
            var query = new GetRoleByIdQuery(roleId);
            var result = await _mediator.Send(query);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Role not found: {RoleId}", roleId);
                return NotFound(result);
            }
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting role: {RoleId}", roleId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error retrieving role" });
        }
    }
    [HttpGet]
    [ProducesResponseType(typeof(Result<List<RoleDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAllRoles([FromBody] PagedRequest request)
    {
        _logger.LogInformation("Getting all roles");
        try
        {
            var query = new ListRolesQuery(request);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting roles");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error retrieving roles" });
        }
    }
    [HttpGet("by-name/{name}")]
    [ProducesResponseType(typeof(Result<RoleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetByName(string name)
    {
        _logger.LogInformation("Getting role by name: {RoleName}", name);
        try
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest(new { message = "Role name is required" });
            var query = new GetRoleByNameQuery(name);
            var result = await _mediator.Send(query);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Role not found by name: {RoleName}", name);
                return NotFound(result);
            }
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting role by name");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error retrieving role" });
        }
    }
    [HttpPost]
    [ProducesResponseType(typeof(Result<RoleDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest request)
    {
        _logger.LogInformation("Creating new role: {RoleName}", request.RoleName);
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var command = new CreateRoleCommand(request.RoleName, request.Description, (RoleType)request.RoleType);
            var result = await _mediator.Send(command);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Role creation failed: {RoleName}", request.RoleName);
                return BadRequest(result);
            }
            _logger.LogInformation("Role successfully created: {RoleName}", request.RoleName);
            return CreatedAtAction(nameof(GetRole), new { roleId = result.Value?.Id }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating role");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error creating role" });
        }
    }
    [HttpPut("{roleId}")]
    [ProducesResponseType(typeof(Result<RoleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateRole(Guid roleId, [FromBody] UpdateRoleRequest request)
    {
        _logger.LogInformation("Updating role: {RoleId}", roleId);
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var command = new UpdateRoleCommand(roleId, request.RoleName, request.Description);
            var result = await _mediator.Send(command);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Role update failed: {RoleId}", roleId);
                return BadRequest(result);
            }
            _logger.LogInformation("Role successfully updated: {RoleId}", roleId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating role: {RoleId}", roleId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error updating role" });
        }
    }
    [HttpDelete("{roleId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteRole(Guid roleId)
    {
        _logger.LogInformation("Deleting role: {RoleId}", roleId);
        try
        {
            var command = new DeleteRoleCommand(roleId);
            var result = await _mediator.Send(command);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Role deletion failed: {RoleId}", roleId);
                return NotFound(result);
            }
            _logger.LogInformation("Role successfully deleted: {RoleId}", roleId);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting role: {RoleId}", roleId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error deleting role" });
        }
    }
    [HttpGet("{roleId}/permissions")]
    [ProducesResponseType(typeof(Result<List<PermissionDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetRolePermissions(Guid roleId)
    {
        _logger.LogInformation("Getting permissions for role: {RoleId}", roleId);
        try
        {
            var query = new GetRolePermissionsQuery(roleId);
            var result = await _mediator.Send(query);
            if (!result.IsSuccess)
                return NotFound(result);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting role permissions: {RoleId}", roleId);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Error retrieving role permissions" });
        }
    }
    [HttpPost("{roleId}/permissions")]
    [ProducesResponseType(typeof(Result<RoleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AssignPermission(Guid roleId, [FromBody] AssignPermissionToRoleRequest request)
    {
        _logger.LogInformation("Assigning permission {PermissionId} to role {RoleId}", request.PermissionId, roleId);
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var command = new AssignPermissionToRoleCommand(roleId, request.PermissionId);
            var result = await _mediator.Send(command);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Permission assignment failed for role: {RoleId}", roleId);
                return BadRequest(result);
            }
            _logger.LogInformation("Permission successfully assigned to role: {RoleId}", roleId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning permission to role: {RoleId}", roleId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error assigning permission" });
        }
    }
    [HttpDelete("{roleId}/permissions/{permissionId}")]
    [ProducesResponseType(typeof(Result<RoleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RemovePermission(Guid roleId, Guid permissionId)
    {
        _logger.LogInformation("Removing permission {PermissionId} from role {RoleId}", permissionId, roleId);
        try
        {
            var command = new RemovePermissionFromRoleCommand(roleId, permissionId);
            var result = await _mediator.Send(command);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Permission removal failed for role: {RoleId}", roleId);
                return NotFound(result);
            }
            _logger.LogInformation("Permission successfully removed from role: {RoleId}", roleId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing permission from role: {RoleId}", roleId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error removing permission" });
        }
    }
    [HttpPost("{roleId}/activate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ActivateRole(Guid roleId)
    {
        _logger.LogInformation("Activating role: {RoleId}", roleId);
        try
        {
            var command = new ActivateRoleCommand(roleId);
            var result = await _mediator.Send(command);
            if (!result.IsSuccess)
                return NotFound(result);
            return Ok(new { success = true, message = "Role activated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error activating role: {RoleId}", roleId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error activating role" });
        }
    }
    [HttpPost("{roleId}/deactivate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeactivateRole(Guid roleId)
    {
        _logger.LogInformation("Deactivating role: {RoleId}", roleId);
        try
        {
            var command = new DeactivateRoleCommand(roleId);
            var result = await _mediator.Send(command);
            if (!result.IsSuccess)
                return NotFound(result);
            return Ok(new { success = true, message = "Role deactivated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deactivating role: {RoleId}", roleId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error deactivating role" });
        }
    }
}