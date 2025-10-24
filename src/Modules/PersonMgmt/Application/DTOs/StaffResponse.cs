namespace PersonMgmt.Application.DTOs;
public class StaffResponse
{
    public Guid PersonId { get; set; }
    public string EmployeeNumber { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public DateTime HireDate { get; set; }
    public Guid DepartmentId { get; set; }
    public decimal? Salary { get; set; }
    public string EmploymentStatus { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}