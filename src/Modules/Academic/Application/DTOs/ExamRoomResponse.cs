namespace Academic.Application.DTOs;

public class ExamRoomResponse
{
    public Guid Id { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public string Building { get; set; } = string.Empty;
    public int Capacity { get; set; }
}