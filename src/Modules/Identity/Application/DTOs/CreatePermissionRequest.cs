namespace Identity.Application.DTOs;
public class CreatePermissionRequest
{
    public string PermissionName { get; set; } = string.Empty;
    public int PermissionType { get; set; }
    public string Description { get; set; } = string.Empty;
}