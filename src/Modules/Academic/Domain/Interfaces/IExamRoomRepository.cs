using Academic.Domain.Aggregates;
using Core.Domain.Repositories;

namespace Academic.Domain.Interfaces;

/// <summary>
/// Repository interface for ExamRoom entity
/// </summary>
public interface IExamRoomRepository : IRepository<ExamRoom>
{
    Task<ExamRoom?> GetByRoomNumberAsync(string roomNumber, CancellationToken ct = default);
    Task<IEnumerable<ExamRoom>> GetActiveRoomsAsync(CancellationToken ct = default);
    Task<IEnumerable<ExamRoom>> GetByBuildingAsync(string building, CancellationToken ct = default);
    Task<IEnumerable<ExamRoom>> GetByCapacityAsync(int minCapacity, CancellationToken ct = default);
}