namespace API.Middlewares;

/// <summary>
/// Error response model
/// </summary>
public class ErrorResponse
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public Dictionary<string, string[]>? Errors { get; set; }
}