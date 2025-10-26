using Academic.Domain.Aggregates;
using Academic.Domain.Interfaces;
using Academic.Domain.Specifications;
using Shared.Infrastructure.Persistence.Contexts;

namespace Shared.Infrastructure.Persistence.Repositories.Academic;

/// <summary>
/// Repository for ExamRoom entity
/// </summary>
public class ExamRoomRepository : GenericRepository<ExamRoom>, IExamRoomRepository
{
    public ExamRoomRepository(AppDbContext context) : base(context) { }

    public async Task<ExamRoom?> GetByRoomNumberAsync(string roomNumber, CancellationToken ct = default)
    {
        var spec = new ExamRoomByRoomNumberSpec(roomNumber);
        return await GetAsync(spec, ct);
    }

    public async Task<IEnumerable<ExamRoom>> GetActiveRoomsAsync(CancellationToken ct = default)
    {
        var spec = new ActiveExamRoomsSpec();
        var result = await GetAllAsync(spec, ct);
        return result.Data;
    }

    public async Task<IEnumerable<ExamRoom>> GetByBuildingAsync(string building, CancellationToken ct = default)
    {
        var spec = new ExamRoomsByBuildingSpec(building);
        var result = await GetAllAsync(spec, ct);
        return result.Data;
    }

    public async Task<IEnumerable<ExamRoom>> GetByCapacityAsync(int minCapacity, CancellationToken ct = default)
    {
        var spec = new ExamRoomsByCapacitySpec(minCapacity);
        var result = await GetAllAsync(spec, ct);
        return result.Data;
    }
}