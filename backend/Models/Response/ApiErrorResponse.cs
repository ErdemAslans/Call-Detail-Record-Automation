namespace Cdr.Api.Models.Response;

/// <summary>
/// Standardized API error response format (FR-034).
/// </summary>
public class ApiErrorResponse
{
    /// <summary>
    /// Error code (e.g., ValidationError, NotFound, Unauthorized)
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Human-readable error message
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Additional error details (optional)
    /// </summary>
    public object? Details { get; set; }

    /// <summary>
    /// ISO8601 timestamp when the error occurred
    /// </summary>
    public DateTime Timestamp { get; set; }
}
