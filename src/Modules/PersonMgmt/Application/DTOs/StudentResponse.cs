namespace PersonMgmt.Application.DTOs;

/// <summary>
/// Öğrenci bilgilerini response vermek için
/// </summary>
public class StudentResponse
{
    /// <summary>
    /// Kişi ID
    /// </summary>
    public Guid PersonId { get; set; }

    /// <summary>
    /// Öğrenci numarası
    /// </summary>
    public string StudentNumber { get; set; } = string.Empty;

    /// <summary>
    /// Program ID
    /// </summary>
    public Guid ProgramId { get; set; }

    /// <summary>
    /// Kayıt tarihi
    /// </summary>
    public DateTime EnrollmentDate { get; set; }

    /// <summary>
    /// Eğitim düzeyi
    /// </summary>
    public string EducationLevel { get; set; } = string.Empty;

    /// <summary>
    /// Öğrenci durumu
    /// </summary>
    public string StudentStatus { get; set; } = string.Empty;

    /// <summary>
    /// GPA (not ortalaması)
    /// </summary>
    public decimal? GPA { get; set; }

    /// <summary>
    /// Oluşturulma tarihi
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Güncellenme tarihi
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}