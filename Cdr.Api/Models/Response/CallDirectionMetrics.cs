namespace Cdr.Api.Models.Response;

/// <summary>
/// Metrics by call direction (incoming/outgoing/internal)
/// </summary>
public class CallDirectionMetrics
{
    public int TotalCalls { get; set; }
    public int AnsweredCalls { get; set; }
    public int MissedCalls { get; set; }
    public int AbandonedCalls { get; set; }
    public TimeSpan TotalDuration { get; set; }
    public TimeSpan AverageDuration { get; set; }
    public double AnswerRate { get; set; }
}
