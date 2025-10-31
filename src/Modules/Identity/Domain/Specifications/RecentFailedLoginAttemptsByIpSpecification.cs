using Core.Domain.Specifications;
using Identity.Domain.Aggregates;

namespace Identity.Domain.Specifications;

public class RecentFailedLoginAttemptsByIpSpecification : Specification<FailedLoginAttempt>
{
    public RecentFailedLoginAttemptsByIpSpecification(string ipAddress, int minutesBack = 60)
    {
        var cutoffTime = DateTime.UtcNow.AddMinutes(-minutesBack);

        Criteria = fla => fla.IpAddress == ipAddress
                          && fla.AttemptedAt > cutoffTime;

        AddOrderByDescending(fla => fla.AttemptedAt);
    }
}