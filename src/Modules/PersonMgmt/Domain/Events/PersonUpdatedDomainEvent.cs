using Core.Domain.Events;

namespace PersonMgmt.Domain.Events;

/// <summary>
/// Person bilgileri güncellendiğinde raise edilen event
/// </summary>
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
        DateTime updatedAt)
    {
        PersonId = personId;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        UpdatedAt = updatedAt;
    }
}