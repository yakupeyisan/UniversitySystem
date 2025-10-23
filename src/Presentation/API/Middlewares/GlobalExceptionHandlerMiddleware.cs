using System.Text.Json;
using Core.Application.Exceptions;

namespace API.Middlewares;


/// <summary>
/// Global Exception Handler Middleware
/// 
/// Tüm exception'ları catch eder ve standardized response döner
/// </summary>
public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    /// <summary>
    /// Constructor
    /// </summary>
    public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Invoke middleware
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    /// <summary>
    /// Exception'ı handle et ve response gönder
    /// </summary>
    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new ErrorResponse();

        switch (exception)
        {
            case ValidationException validationEx:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                response = new ErrorResponse
                {
                    IsSuccess = false,
                    Message = "Validation error",
                    Errors = validationEx.Errors.ToDictionary()
                };
                break;

            case KeyNotFoundException _:
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                response = new ErrorResponse
                {
                    IsSuccess = false,
                    Message = "Resource not found"
                };
                break;

            case UnauthorizedAccessException _:
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                response = new ErrorResponse
                {
                    IsSuccess = false,
                    Message = "Unauthorized access"
                };
                break;

            case ArgumentException argEx:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                response = new ErrorResponse
                {
                    IsSuccess = false,
                    Message = argEx.Message
                };
                break;

            default:
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                response = new ErrorResponse
                {
                    IsSuccess = false,
                    Message = "An internal server error occurred"
                };
                break;
        }

        var json = JsonSerializer.Serialize(response);
        return context.Response.WriteAsync(json);
    }
}