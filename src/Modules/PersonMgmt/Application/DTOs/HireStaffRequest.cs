namespace PersonMgmt.Application.DTOs;

public class HireStaffRequest
{
    public string EmployeeNumber { get; set; }
    public string Position { get; set; }
    public DateTime HireDate { get; set; }
}