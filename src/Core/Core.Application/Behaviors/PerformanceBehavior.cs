using MediatR;
using Microsoft.Extensions.Logging;
namespace Core.Application.Behaviors;
public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;
    private const int SlowThresholdMs = 500;
    public PerformanceBehavior(ILogger<PerformanceBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }
    public async Task<TResponse> Handle(
    TRequest request,
    RequestHandlerDelegate<TResponse> next,
    CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var startTime = DateTime.UtcNow;
        var response = await next();
        var duration = DateTime.UtcNow - startTime;
        if (duration.TotalMilliseconds > SlowThresholdMs)
        {
            _logger.LogWarning(
                "Long Running Request: {RequestName} - Duration: {DurationMs}ms",
                requestName,
                duration.TotalMilliseconds);
        }
        return response;
    }
}