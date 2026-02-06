namespace Cdr.Api.Models.Response;

/// <summary>
/// Call redirection audit record
/// </summary>
public class RedirectionRecord
{
    public Guid CallId { get; set; }
    public string SourceNumber { get; set; } = string.Empty;
    public string TargetNumber { get; set; } = string.Empty;
    public string SourceOperator { get; set; } = string.Empty;
    public string TargetOperator { get; set; } = string.Empty;
    public DateTime RedirectionTime { get; set; }
    public string RedirectionReason { get; set; } = string.Empty;
}
