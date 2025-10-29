using Core.Domain.Events;

namespace Identity.Domain.Events;

public class UserUpdatedEvent : DomainEvent
{
    public UserUpdatedEvent(Guid userId, string email, string firstName, string lastName)
    {
        UserId = userId;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
    }

    public Guid UserId { get; }
    public string Email { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}