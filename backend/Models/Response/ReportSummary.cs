namespace Cdr.Api.Models.Response;

/// <summary>
/// Overall summary metrics for a report
/// </summary>
public class ReportSummary
{
    public int TotalIncomingCalls { get; set; }
    public int TotalAnsweredCalls { get; set; }
    public int TotalMissedCalls { get; set; }
    public int TotalOutgoingCalls { get; set; }
    public double AnswerRate { get; set; }
    public int WorkHoursCalls { get; set; }
    public int AfterHoursCalls { get; set; }
}
