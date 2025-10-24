using Core.Domain.Events;
namespace PersonMgmt.Domain.Events;
public class PersonUpdatedDomainEvent : DomainEvent
{
    public Guid PersonId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public DateTime UpdatedAt { get; set; }
    public PersonUpdatedDomainEvent(
        Guid personId,
        string firstName,
        string lastName,
        string email,
        DateTime updatedAt) : base(personId)
    {
        PersonId = personId;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        UpdatedAt = updatedAt;
    }
}