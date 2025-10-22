namespace PersonMgmt.Application.DTOs;

/// <summary>
/// Kişiyi öğrenci olarak kaydetmek için request
/// </summary>
public class EnrollStudentRequest
{
    /// <summary>
    /// Öğrenci numarası (unique)
    /// </summary>
    public string StudentNumber { get; set; }

    /// <summary>
    /// Program ID
    /// </summary>
    public Guid ProgramId { get; set; }

    /// <summary>
    /// Kayıt tarihi
    /// </summary>
    public DateTime EnrollmentDate { get; set; }

    /// <summary>
    /// Eğitim düzeyi (0=Lisans, 1=Yüksek Lisans, 2=Doktora)
    /// </summary>
    public byte EducationLevel { get; set; }
}