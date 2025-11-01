using Core.Domain;
namespace Academic.Domain.Aggregates;
public class ExamRoom : AuditableEntity
{
    private ExamRoom()
    {
    }
    public string RoomNumber { get; private set; } = null!;
    public string Building { get; private set; } = null!;
    public int Floor { get; private set; }
    public int Capacity { get; private set; }
    public bool IsActive { get; private set; }
    public static ExamRoom Create(
        string roomNumber,
        string building,
        int floor,
        int capacity)
    {
        if (string.IsNullOrWhiteSpace(roomNumber))
            throw new ArgumentException("Room number cannot be empty");
        if (string.IsNullOrWhiteSpace(building))
            throw new ArgumentException("Building cannot be empty");
        if (floor < 0)
            throw new ArgumentException("Floor cannot be negative");
        if (capacity <= 0)
            throw new ArgumentException("Capacity must be greater than 0");
        var room = new ExamRoom
        {
            Id = Guid.NewGuid(),
            RoomNumber = roomNumber,
            Building = building,
            Floor = floor,
            Capacity = capacity,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        return room;
    }
    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }
    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
    public void UpdateCapacity(int newCapacity)
    {
        if (newCapacity <= 0)
            throw new ArgumentException("Capacity must be greater than 0");
        Capacity = newCapacity;
        UpdatedAt = DateTime.UtcNow;
    }
    public bool CanAccommodate(int studentCount)
    {
        return studentCount <= Capacity;
    }
    public override string ToString()
    {
        return $"{Building} - Room {RoomNumber} (Floor {Floor}, Capacity: {Capacity})";
    }
}