using Academic.Domain.Enums;
using Academic.Domain.Events;
using Core.Domain;
using Core.Domain.Specifications;
namespace Academic.Domain.Aggregates;
public class PrerequisiteWaiver : AuditableEntity, ISoftDelete
{
    private PrerequisiteWaiver()
    {
    }
    public Guid StudentId { get; private set; }
    public Guid PrerequisiteId { get; private set; }
    public Guid CourseId { get; private set; }
    public DateTime RequestedDate { get; private set; }
    public string Reason { get; private set; } = null!;
    public PrerequisiteWaiverStatus Status { get; private set; }
    public Guid? ApprovedBy { get; private set; }
    public DateTime? ApprovedDate { get; private set; }
    public string? ApprovalNotes { get; private set; }
    public DateTime? ExpiryDate { get; private set; }
    public Prerequisite? Prerequisite { get; private set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }
    public void Delete(Guid deletedBy)
    {
        throw new NotImplementedException();
    }
    public void Restore()
    {
        throw new NotImplementedException();
    }
    public static PrerequisiteWaiver Create(
        Guid studentId,
        Guid prerequisiteId,
        Guid courseId,
        string reason,
        int validityDays = 180)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Waiver reason cannot be empty");
        if (reason.Length > 1000)
            throw new ArgumentException("Waiver reason cannot exceed 1000 characters");
        if (validityDays <= 0)
            throw new ArgumentException("Validity days must be greater than 0");
        var waiver = new PrerequisiteWaiver
        {
            Id = Guid.NewGuid(),
            StudentId = studentId,
            PrerequisiteId = prerequisiteId,
            CourseId = courseId,
            RequestedDate = DateTime.UtcNow,
            Reason = reason,
            Status = PrerequisiteWaiverStatus.Submitted,
            ExpiryDate = DateTime.UtcNow.AddDays(validityDays),
            CreatedAt = DateTime.UtcNow
        };
        return waiver;
    }
    public void SubmitForReview()
    {
        if (Status != PrerequisiteWaiverStatus.Submitted)
            throw new InvalidOperationException("Waiver is not in submitted status");
        Status = PrerequisiteWaiverStatus.UnderReview;
        UpdatedAt = DateTime.UtcNow;
    }
    public void Approve(Guid approvedBy, string? notes = null, int validityDays = 180)
    {
        if (Status == PrerequisiteWaiverStatus.Approved)
            throw new InvalidOperationException("Waiver is already approved");
        Status = PrerequisiteWaiverStatus.Approved;
        ApprovedBy = approvedBy;
        ApprovedDate = DateTime.UtcNow;
        ApprovalNotes = notes;
        ExpiryDate = DateTime.UtcNow.AddDays(validityDays);
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new PrerequisiteWaiverApproved(
            Id,
            StudentId,
            PrerequisiteId,
            CourseId));
    }
    public void Deny(Guid reviewedBy, string? notes = null)
    {
        if (Status == PrerequisiteWaiverStatus.Denied)
            throw new InvalidOperationException("Waiver is already denied");
        Status = PrerequisiteWaiverStatus.Denied;
        ApprovedBy = reviewedBy;
        ApprovalNotes = notes;
        UpdatedAt = DateTime.UtcNow;
    }
    public void Withdraw()
    {
        if (Status == PrerequisiteWaiverStatus.Withdrawn)
            throw new InvalidOperationException("Waiver is already withdrawn");
        Status = PrerequisiteWaiverStatus.Withdrawn;
        UpdatedAt = DateTime.UtcNow;
    }
    public bool IsApproved()
    {
        return Status == PrerequisiteWaiverStatus.Approved;
    }
    public bool IsValid()
    {
        return IsApproved() && DateTime.UtcNow <= ExpiryDate;
    }
    public bool IsExpired()
    {
        return IsApproved() && DateTime.UtcNow > ExpiryDate;
    }
    public bool IsPending()
    {
        return Status == PrerequisiteWaiverStatus.UnderReview;
    }
    public bool CanBeWithdrawn()
    {
        return Status is not (PrerequisiteWaiverStatus.Approved or PrerequisiteWaiverStatus.Withdrawn);
    }
}