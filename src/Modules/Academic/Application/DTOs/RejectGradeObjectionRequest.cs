namespace Academic.Application.DTOs;

/// <summary>
/// Request to reject a grade objection
/// Kullanýcý: Typically instructor/admin
/// </summary>
public class RejectGradeObjectionRequest
{
    /// <summary>
    /// ID of the reviewer (instructor/admin) who is rejecting the objection
    /// </summary>
    public Guid ReviewedBy { get; set; }

    /// <summary>
    /// Reason for rejecting the grade objection
    /// </summary>
    public string RejectionReason { get; set; } = string.Empty;

    /// <summary>
    /// Additional notes/comments for the student (optional)
    /// </summary>
    public string? AdditionalNotes { get; set; }
}


