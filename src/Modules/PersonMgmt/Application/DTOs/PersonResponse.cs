namespace PersonMgmt.Application.DTOs;

public class PersonResponse
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string IdentificationNumber { get; set; }
    public DateTime BirthDate { get; set; }
    public string Gender { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public Guid? DepartmentId { get; set; }
    public string? ProfilePhotoUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsStudent { get; set; }
    public bool IsStaff { get; set; }
    public int ActiveRestrictionCount { get; set; }
}