using Core.Domain.Exceptions;

namespace Academic.Domain.Exceptions;

/// <summary>
/// Thrown when waiting list position is invalid
/// </summary>
public class InvalidWaitingListPositionException : DomainException
{
    public override string ErrorCode => "ACD004";
    public override int StatusCode => 400;

    public InvalidWaitingListPositionException(int position)
        : base($"Waiting list position {position} is invalid. Position must be greater than 0.") { }
}