namespace API.Middlewares;
public class RequestLoggingMiddleware
{
    private readonly ILogger<RequestLoggingMiddleware> _logger;
    private readonly RequestDelegate _next;
    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    public async Task InvokeAsync(HttpContext context)
    {
        var startTime = DateTime.UtcNow;
        _logger.LogInformation(
            "HTTP Request: {Method} {Path} - IP: {IP}",
            context.Request.Method,
            context.Request.Path,
            context.Connection.RemoteIpAddress);
        await _next(context);
        var duration = DateTime.UtcNow - startTime;
        _logger.LogInformation(
            "HTTP Response: {StatusCode} - Duration: {Duration}ms",
            context.Response.StatusCode,
            duration.TotalMilliseconds);
    }
}