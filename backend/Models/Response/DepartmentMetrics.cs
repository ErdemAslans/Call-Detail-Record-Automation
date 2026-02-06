namespace Cdr.Api.Models.Response;

/// <summary>
/// Per-department metrics
/// </summary>
public class DepartmentMetrics
{
    public string DepartmentName { get; set; } = string.Empty;
    public int IncomingCalls { get; set; }
    public int AnsweredCalls { get; set; }
    public int MissedCalls { get; set; }
    public int OutgoingCalls { get; set; }
    public double AnswerRate { get; set; }
    public TimeSpan TotalTalkDuration { get; set; }
    public TimeSpan AverageTalkDuration { get; set; }
    public int Redirections { get; set; }
}
