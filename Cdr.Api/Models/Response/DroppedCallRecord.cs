namespace Cdr.Api.Models.Response;

/// <summary>
/// Dropped or failed call record
/// </summary>
public class DroppedCallRecord
{
    public Guid CallId { get; set; }
    public string SourceNumber { get; set; } = string.Empty;
    public string TargetNumber { get; set; } = string.Empty;
    public DateTime CallTime { get; set; }
    public string FailureReason { get; set; } = string.Empty; // "NoAnswer", "Busy", "NetworkError", etc.
    public string Department { get; set; } = string.Empty;
}
