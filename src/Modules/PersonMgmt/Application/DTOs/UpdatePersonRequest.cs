namespace PersonMgmt.Application.DTOs;
public class UpdatePersonRequest
{
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public Guid? DepartmentId { get; set; }
    public string? ProfilePhotoUrl { get; set; }
}