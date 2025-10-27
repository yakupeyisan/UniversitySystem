using Core.Domain.Exceptions;
namespace Academic.Domain.Exceptions;
public class BusinessRuleViolatedException : DomainException
{
    public override string ErrorCode => "ACD011";
    public override int StatusCode => 422;
    public BusinessRuleViolatedException(string ruleName, string message)
        : base($"Business rule '{ruleName}' violated: {message}") { }
}