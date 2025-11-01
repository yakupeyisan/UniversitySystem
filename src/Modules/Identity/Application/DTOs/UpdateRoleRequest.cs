namespace Identity.Application.DTOs;
public class UpdateRoleRequest
{
    public string RoleName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}