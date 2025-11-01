using Core.Domain.Specifications;
using Identity.Domain.Aggregates;
namespace Identity.Domain.Specifications;
public class RecentFailedLoginAttemptsSpecification : Specification<FailedLoginAttempt>
{
    public RecentFailedLoginAttemptsSpecification(string email, int minutesBack = 60)
    {
        var cutoffTime = DateTime.UtcNow.AddMinutes(-minutesBack);
        Criteria = fla => fla.AttemptedEmail == email.ToLowerInvariant()
                          && fla.AttemptedAt > cutoffTime;
        AddOrderByDescending(fla => fla.AttemptedAt);
    }
}