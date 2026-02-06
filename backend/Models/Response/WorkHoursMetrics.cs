namespace Cdr.Api.Models.Response;

/// <summary>
/// Metrics for work hours vs after-hours traffic
/// </summary>
public class WorkHoursMetrics
{
    public int TotalCalls { get; set; }
    public int AnsweredCalls { get; set; }
    public int MissedCalls { get; set; }
    public TimeSpan TotalDuration { get; set; }
    public TimeSpan AverageDuration { get; set; }
    public double AnswerRate { get; set; }
    public string TimeCategory { get; set; } = string.Empty; // "WorkHours" or "AfterHours"
}
