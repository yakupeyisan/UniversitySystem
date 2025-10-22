using Core.Domain.Events;

namespace PersonMgmt.Domain.Events;

// Not: Bu event'ler Core.Domain'deki DomainEvent base class'ından inherit etmelidir
// Burada sadece PersonMgmt'e özel event'ler tanımlanmıştır

/// <summary>
/// Yeni bir person oluşturulduğunda raise edilen event
/// </summary>
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
        DateTime createdAt)
    {
        PersonId = personId;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        CreatedAt = createdAt;
    }
}