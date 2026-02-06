namespace Cdr.Api.Models.Response;

/// <summary>
/// Operator break record details
/// </summary>
public class BreakRecord
{
    public Guid OperatorId { get; set; }
    public string OperatorName { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string BreakType { get; set; } = string.Empty; // "Lunch", "Meeting", etc.
    public TimeSpan Duration { get; set; }
}
