namespace PersonMgmt.Application.DTOs;

/// <summary>
/// Kişiyi personel olarak işe almak için request
/// </summary>
public class HireStaffRequest
{
    /// <summary>
    /// Personel numarası (unique)
    /// </summary>
    public string EmployeeNumber { get; set; }

    /// <summary>
    /// Pozisyon/Ünvan
    /// </summary>
    public string Position { get; set; }

    /// <summary>
    /// İşe alınma tarihi
    /// </summary>
    public DateTime HireDate { get; set; }

    /// <summary>
    /// Departman ID
    /// </summary>
    public Guid DepartmentId { get; set; }

    /// <summary>
    /// Maaş (opsiyonel)
    /// </summary>
    public decimal? Salary { get; set; }
}