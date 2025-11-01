namespace Core.Domain;
public abstract class AuditableEntity : Entity, IAuditableEntity
{
    protected AuditableEntity()
    {
        CreatedAt = DateTime.UtcNow;
    }
    protected AuditableEntity(Guid id) : base(id)
    {
        CreatedAt = DateTime.UtcNow;
    }
    public DateTime CreatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
}