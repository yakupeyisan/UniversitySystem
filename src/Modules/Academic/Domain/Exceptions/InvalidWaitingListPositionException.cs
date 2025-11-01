using Core.Domain.Exceptions;
namespace Academic.Domain.Exceptions;
public class InvalidWaitingListPositionException : DomainException
{
    public InvalidWaitingListPositionException(int position)
        : base($"Waiting list position {position} is invalid. Position must be greater than 0.")
    {
    }
    public override string ErrorCode => "ACD004";
    public override int StatusCode => 400;
}