using Core.Domain.Events;
namespace PersonMgmt.Domain.Events;
public class PersonDeletedDomainEvent : DomainEvent
{
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
    public Guid PersonId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime DeletedAt { get; set; }
}