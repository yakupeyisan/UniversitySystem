using Academic.Domain.Enums;
using Academic.Domain.Events;
using Academic.Domain.ValueObjects;
using Core.Domain;
using Core.Domain.Specifications;

namespace Academic.Domain.Aggregates;

/// <summary>
/// Exam aggregate representing an academic exam for a course
/// </summary>
public class Exam : AuditableEntity, ISoftDelete
{
    public Guid CourseId { get; private set; }
    public ExamType ExamType { get; private set; }
    public DateOnly ExamDate { get; private set; }
    public TimeSlot TimeSlot { get; private set; } = null!;
    public Guid? ExamRoomId { get; private set; }
    public int MaxCapacity { get; private set; }
    public int CurrentRegisteredCount { get; private set; }
    public ExamStatus Status { get; private set; }
    public bool IsOnline { get; private set; }
    public string? OnlineLink { get; private set; }

    // Soft delete
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }
    public Course? Course { get; private set; }
    public void Delete(Guid deletedBy)
    {
        throw new NotImplementedException();
    }

    public void Restore()
    {
        throw new NotImplementedException();
    }

    private Exam() { }

    public static Exam Create(
        Guid courseId,
        ExamType examType,
        DateOnly examDate,
        TimeSlot timeSlot,
        int maxCapacity,
        Guid? examRoomId = null,
        bool isOnline = false,
        string? onlineLink = null)
    {
        if (maxCapacity <= 0)
            throw new ArgumentException("Max capacity must be greater than 0");

        if (examDate < DateOnly.FromDateTime(DateTime.UtcNow))
            throw new ArgumentException("Exam date cannot be in the past");

        if (isOnline && string.IsNullOrWhiteSpace(onlineLink))
            throw new ArgumentException("Online link must be provided for online exams");

        var exam = new Exam
        {
            Id = Guid.NewGuid(),
            CourseId = courseId,
            ExamType = examType,
            ExamDate = examDate,
            TimeSlot = timeSlot,
            ExamRoomId = examRoomId,
            MaxCapacity = maxCapacity,
            CurrentRegisteredCount = 0,
            Status = ExamStatus.Scheduled,
            IsOnline = isOnline,
            OnlineLink = onlineLink,
            CreatedAt = DateTime.UtcNow
        };

        exam.AddDomainEvent(new ExamScheduled(
            exam.Id,
            courseId,
            examType,
            examDate,
            timeSlot));

        return exam;
    }

    public void RegisterStudent()
    {
        if (CurrentRegisteredCount >= MaxCapacity)
            throw new InvalidOperationException("Exam capacity is full");

        CurrentRegisteredCount++;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UnregisterStudent()
    {
        if (CurrentRegisteredCount <= 0)
            throw new InvalidOperationException("Cannot unregister - no students registered");

        CurrentRegisteredCount--;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Start()
    {
        if (Status != ExamStatus.Scheduled)
            throw new InvalidOperationException("Only scheduled exams can be started");

        Status = ExamStatus.InProgress;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Complete()
    {
        if (Status != ExamStatus.InProgress)
            throw new InvalidOperationException("Only in-progress exams can be completed");

        Status = ExamStatus.Completed;
        UpdatedAt = DateTime.UtcNow;
    }

    public void StartGrading()
    {
        if (Status != ExamStatus.Completed)
            throw new InvalidOperationException("Exam must be completed before grading");

        Status = ExamStatus.Grading;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel(string reason)
    {
        if (Status == ExamStatus.Cancelled)
            throw new InvalidOperationException("Exam is already cancelled");

        Status = ExamStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsCapacityFull() => CurrentRegisteredCount >= MaxCapacity;

    public bool HasAvailableSlots() => CurrentRegisteredCount < MaxCapacity;

    public bool IsFutureExam() => ExamDate > DateOnly.FromDateTime(DateTime.UtcNow);

    public bool ConflictsWith(Exam other)
    {
        if (ExamDate != other.ExamDate)
            return false;

        return TimeSlot.ConflictsWith(other.TimeSlot);
    }

    public void UpdateExamRoom(Guid? newExamRoomId)
    {
        ExamRoomId = newExamRoomId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateOnlineLink(string? newLink)
    {
        if (IsOnline && string.IsNullOrWhiteSpace(newLink))
            throw new ArgumentException("Online link cannot be empty for online exams");

        OnlineLink = newLink;
        UpdatedAt = DateTime.UtcNow;
    }
}