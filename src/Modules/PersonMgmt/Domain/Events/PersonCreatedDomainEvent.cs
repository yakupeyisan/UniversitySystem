using Core.Domain.Events;
namespace PersonMgmt.Domain.Events;
public class PersonCreatedDomainEvent : DomainEvent
{
    public Guid PersonId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public DateTime CreatedAt { get; set; }
    public PersonCreatedDomainEvent(
        Guid personId,
        string firstName,
        string lastName,
        string email,
        DateTime createdAt) : base(personId)
    {
        PersonId = personId;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        CreatedAt = createdAt;
    }
}