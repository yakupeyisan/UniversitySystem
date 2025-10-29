namespace PersonMgmt.Application.DTOs;

public class CreatePersonRequest
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string IdentificationNumber { get; set; }
    public DateTime BirthDate { get; set; }
    public byte Gender { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public Guid? DepartmentId { get; set; }
    public string? ProfilePhotoUrl { get; set; }
}