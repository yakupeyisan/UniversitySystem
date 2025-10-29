namespace Core.Domain.Specifications;

public interface ISoftDelete
{
    bool IsDeleted { get; }
    DateTime? DeletedAt { get; }
    Guid? DeletedBy { get; }
    void Delete(Guid deletedBy);
    void Restore();
}