namespace Identity.Application.DTOs;
public class CreateRoleRequest
{
    public string RoleName { get; set; } = string.Empty;
    public int RoleType { get; set; }
    public string Description { get; set; } = string.Empty;
}