using MediatR;
using Microsoft.Extensions.Logging;

namespace Core.Application.Behaviors;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        try
        {
            _logger.LogInformation("Handling Request: {RequestName}", requestName);
            _logger.LogDebug("Request Data: {@Request}", request);
            var startTime = DateTime.UtcNow;
            var response = await next();
            var duration = DateTime.UtcNow - startTime;
            _logger.LogInformation(
                "Request Completed: {RequestName} - Duration: {DurationMs}ms",
                requestName,
                duration.TotalMilliseconds);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Request Failed: {RequestName} - Exception: {ExceptionMessage}",
                requestName,
                ex.Message);
            throw;
        }
    }
}