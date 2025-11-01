namespace Identity.Application.DTOs;
public class UpdatePermissionRequest
{
    public string PermissionName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}