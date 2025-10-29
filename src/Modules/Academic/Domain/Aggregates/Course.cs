using Academic.Domain.Enums;
using Academic.Domain.Events;
using Academic.Domain.ValueObjects;
using Core.Domain;
using Core.Domain.Specifications;

namespace Academic.Domain.Aggregates;

public class Course : AuditableEntity, ISoftDelete
{
    private readonly List<Guid> _instructorIds = new();
    private readonly List<Guid> _prerequisiteIds = new();

    private Course()
    {
    }

    public CourseCode Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public int ECTS { get; private set; }
    public int Credits { get; private set; }
    public CourseLevel Level { get; private set; }
    public CourseType Type { get; private set; }
    public CourseSemester Semester { get; private set; }
    public int Year { get; private set; }
    public Guid DepartmentId { get; private set; }
    public CourseStatus Status { get; private set; }
    public CapacityInfo Capacity { get; private set; } = null!;
    public IReadOnlyList<Guid> InstructorIds => _instructorIds.AsReadOnly();
    public IReadOnlyList<Guid> PrerequisiteIds => _prerequisiteIds.AsReadOnly();
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public Guid? DeletedBy { get; private set; }

    public void Delete(Guid deletedBy)
    {
        if (IsDeleted)
            throw new InvalidOperationException("Course is already deleted");
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = deletedBy;
    }

    public void Restore()
    {
        if (!IsDeleted)
            throw new InvalidOperationException("Course is not deleted");
        IsDeleted = false;
        DeletedAt = null;
        DeletedBy = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public static Course Create(
        CourseCode code,
        string name,
        int ects,
        int credits,
        CourseLevel level,
        CourseType type,
        CourseSemester semester,
        int year,
        Guid departmentId,
        int maxCapacity,
        string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Course name cannot be empty");
        if (ects <= 0 || ects > 20)
            throw new ArgumentException("ECTS must be between 1 and 20");
        if (credits <= 0)
            throw new ArgumentException("Credits must be greater than 0");
        if (maxCapacity <= 0)
            throw new ArgumentException("Max capacity must be greater than 0");
        var course = new Course
        {
            Id = Guid.NewGuid(),
            Code = code,
            Name = name,
            Description = description,
            ECTS = ects,
            Credits = credits,
            Level = level,
            Type = type,
            Semester = semester,
            Year = year,
            DepartmentId = departmentId,
            Status = CourseStatus.Active,
            Capacity = CapacityInfo.Create(maxCapacity),
            CreatedAt = DateTime.UtcNow
        };
        course.AddDomainEvent(new CourseCreated(course.Id, code.Value, name, semester));
        return course;
    }

    public void AddInstructor(Guid instructorId)
    {
        if (_instructorIds.Contains(instructorId))
            throw new InvalidOperationException("Instructor already assigned to this course");
        _instructorIds.Add(instructorId);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveInstructor(Guid instructorId)
    {
        if (!_instructorIds.Remove(instructorId))
            throw new InvalidOperationException("Instructor not found in course");
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddPrerequisite(Guid prerequisiteId)
    {
        if (_prerequisiteIds.Contains(prerequisiteId))
            throw new InvalidOperationException("Prerequisite already added to this course");
        _prerequisiteIds.Add(prerequisiteId);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemovePrerequisite(Guid prerequisiteId)
    {
        if (!_prerequisiteIds.Remove(prerequisiteId))
            throw new InvalidOperationException("Prerequisite not found in course");
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsCapacityFull()
    {
        return Capacity.IsFull();
    }

    public bool HasAvailableSeats()
    {
        return Capacity.HasAvailableSeats();
    }

    public bool HasPrerequisites()
    {
        return _prerequisiteIds.Any();
    }

    public void IncrementEnrollment()
    {
        if (IsCapacityFull())
            throw new InvalidOperationException("Course capacity is full");
        Capacity = Capacity.WithIncrementedEnrollment();
        UpdatedAt = DateTime.UtcNow;
    }

    public void DecrementEnrollment()
    {
        Capacity = Capacity.WithDecrementedEnrollment();
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel(string reason)
    {
        if (Status == CourseStatus.Cancelled)
            throw new InvalidOperationException("Course is already cancelled");
        Status = CourseStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        if (Status == CourseStatus.Active)
            throw new InvalidOperationException("Course is already active");
        Status = CourseStatus.Active;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        if (Status == CourseStatus.Inactive)
            throw new InvalidOperationException("Course is already inactive");
        Status = CourseStatus.Inactive;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Update(
        string name,
        string? description,
        int ects,
        int credits,
        int maxCapacity)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Course name cannot be empty");
        if (ects <= 0 || ects > 20)
            throw new ArgumentException("ECTS must be between 1 and 20");
        if (credits <= 0)
            throw new ArgumentException("Credits must be greater than 0");
        if (maxCapacity <= 0)
            throw new ArgumentException("Max capacity must be greater than 0");
        Name = name;
        Description = description;
        ECTS = ects;
        Credits = credits;
        Capacity = Capacity.WithUpdatedCapacity(maxCapacity);
        UpdatedAt = DateTime.UtcNow;
    }
}