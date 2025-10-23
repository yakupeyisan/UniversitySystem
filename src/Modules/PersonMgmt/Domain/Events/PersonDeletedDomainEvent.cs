using Core.Domain.Events;

namespace PersonMgmt.Domain.Events;

/// <summary>
/// Person silindiğinde (soft delete) raise edilen event
/// </summary>
public class PersonDeletedDomainEvent : DomainEvent
{
    public Guid PersonId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime DeletedAt { get; set; }

    public PersonDeletedDomainEvent(
        Guid personId,
        string firstName,
        string lastName,
        DateTime deletedAt) : base(personId)  
    {
        PersonId = personId;
        FirstName = firstName;
        LastName = lastName;
        DeletedAt = deletedAt;
    }
}
