using Academic.Domain.Enums;
using Academic.Domain.Events;
using Academic.Domain.ValueObjects;
using Core.Domain;
using Core.Domain.Specifications;

namespace Academic.Domain.Aggregates;

public class Exam : AuditableEntity, ISoftDelete
{
    private Exam()
    {
    }

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
    public Course? Course { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public Guid? DeletedBy { get; private set; }

    public void Delete(Guid deletedBy)
    {
        if (IsDeleted)
            throw new InvalidOperationException("Exam is already deleted");
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = deletedBy;
    }

    public void Restore()
    {
        if (!IsDeleted)
            throw new InvalidOperationException("Exam is not deleted");
        IsDeleted = false;
        DeletedAt = null;
        DeletedBy = null;
        UpdatedAt = DateTime.UtcNow;
    }

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
        if (courseId == Guid.Empty)
            throw new ArgumentException("Course ID cannot be empty");
        if (maxCapacity <= 0)
            throw new ArgumentException("Max capacity must be greater than 0");
        if (examDate < DateOnly.FromDateTime(DateTime.UtcNow))
            throw new ArgumentException("Exam date cannot be in the past");
        if (isOnline && string.IsNullOrWhiteSpace(onlineLink))
            throw new ArgumentException("Online link must be provided for online exams");
        if (!isOnline && examRoomId == null)
            throw new ArgumentException("Exam room must be specified for on-site exams");
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
            throw new InvalidOperationException("Exam is at full capacity");
        if (Status != ExamStatus.Scheduled)
            throw new InvalidOperationException("Exam is not scheduled");
        CurrentRegisteredCount++;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UnregisterStudent()
    {
        if (CurrentRegisteredCount <= 0)
            throw new InvalidOperationException("No students registered for this exam");
        CurrentRegisteredCount--;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Start()
    {
        if (Status != ExamStatus.Scheduled)
            throw new InvalidOperationException("Exam cannot be started from current status");
        Status = ExamStatus.InProgress;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Complete()
    {
        if (Status != ExamStatus.InProgress)
            throw new InvalidOperationException("Exam must be in progress to complete");
        Status = ExamStatus.Completed;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel(string reason)
    {
        if (Status == ExamStatus.Cancelled)
            throw new InvalidOperationException("Exam is already cancelled");
        if (Status == ExamStatus.Completed)
            throw new InvalidOperationException("Cannot cancel a completed exam");
        Status = ExamStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Postpone(DateOnly newDate, TimeSlot newTimeSlot)
    {
        if (Status == ExamStatus.Completed)
            throw new InvalidOperationException("Cannot postpone a completed exam");
        if (Status == ExamStatus.Cancelled)
            throw new InvalidOperationException("Cannot postpone a cancelled exam");
        if (newDate <= DateOnly.FromDateTime(DateTime.UtcNow))
            throw new ArgumentException("New exam date must be in the future");
        ExamDate = newDate;
        TimeSlot = newTimeSlot;
        Status = ExamStatus.Postponed;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool HasAvailableSeats()
    {
        return CurrentRegisteredCount < MaxCapacity;
    }

    public bool IsCapacityFull()
    {
        return CurrentRegisteredCount >= MaxCapacity;
    }

    public bool CanBeRegistered()
    {
        return Status == ExamStatus.Scheduled && HasAvailableSeats();
    }

    public void Update(DateOnly newDate, TimeSlot newTimeSlot, Guid? newExamRoomId = null)
    {
        if (Status != ExamStatus.Scheduled)
            throw new InvalidOperationException("Can only update scheduled exams");
        if (newDate < DateOnly.FromDateTime(DateTime.UtcNow))
            throw new ArgumentException("Exam date cannot be in the past");
        if (!IsOnline && newExamRoomId == null)
            throw new ArgumentException("Exam room must be specified for on-site exams");
        ExamDate = newDate;
        TimeSlot = newTimeSlot;
        if (newExamRoomId.HasValue)
            ExamRoomId = newExamRoomId;
        UpdatedAt = DateTime.UtcNow;
    }
}