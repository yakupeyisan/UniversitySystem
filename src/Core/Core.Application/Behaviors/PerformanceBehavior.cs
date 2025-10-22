using MediatR;
using Microsoft.Extensions.Logging;

namespace Core.Application.Behaviors;

/// <summary>
/// PerformanceBehavior - Slow request detection
/// 
/// Sorumluluğu:
/// - Request execution time'ını measure etme
/// - Yavaş requests'leri log etme (warning level)
/// - Performance monitoring
/// 
/// Konfigürasyon:
/// - SlowThresholdMs: Default 500ms (yapılandırılabilir)
/// 
/// Kullanım (Program.cs):
/// services.AddScoped(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
/// 
/// Log formatı (yavaş request):
/// [WARNING] Long Running Request: GetAllPersonsQuery - Duration: 1250ms
/// 
/// Not: ValidationBehavior ve LoggingBehavior'dan sonra çalışır
/// Sıra önemli: Validation -> Logging -> Performance -> Handler
/// </summary>
public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;

    /// <summary>
    /// Yavaş request threshold (ms)
    /// </summary>
    private const int SlowThresholdMs = 500;

    public PerformanceBehavior(ILogger<PerformanceBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Pipeline'ı handle et (performance monitoring ile)
    /// </summary>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var startTime = DateTime.UtcNow;

        var response = await next();

        var duration = DateTime.UtcNow - startTime;

        // Eğer threshold'u geçtiyse warning log et
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