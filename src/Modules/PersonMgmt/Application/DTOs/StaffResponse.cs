namespace PersonMgmt.Application.DTOs;

/// <summary>
/// Personel bilgilerini response vermek için
/// </summary>
public class StaffResponse
{
    /// <summary>
    /// Kişi ID
    /// </summary>
    public Guid PersonId { get; set; }

    /// <summary>
    /// Personel numarası
    /// </summary>
    public string EmployeeNumber { get; set; } = string.Empty;

    /// <summary>
    /// Pozisyon/Unvan
    /// </summary>
    public string Position { get; set; } = string.Empty;

    /// <summary>
    /// İşe alınma tarihi
    /// </summary>
    public DateTime HireDate { get; set; }

    /// <summary>
    /// Departman ID
    /// </summary>
    public Guid DepartmentId { get; set; }

    /// <summary>
    /// Maaş
    /// </summary>
    public decimal? Salary { get; set; }

    /// <summary>
    /// İstihdam durumu
    /// </summary>
    public string EmploymentStatus { get; set; } = string.Empty;

    /// <summary>
    /// Oluşturulma tarihi
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Güncellenme tarihi
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}