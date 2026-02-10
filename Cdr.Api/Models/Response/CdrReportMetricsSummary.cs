namespace Cdr.Api.Models.Response;

/// <summary>
/// Summary metrics for CDR reports
/// </summary>
public class CdrReportMetricsSummary
{
    public int TotalIncomingCalls { get; set; }
    public int TotalAnsweredCalls { get; set; }
    public int TotalMissedCalls { get; set; }
    public int TotalOutgoingCalls { get; set; }
    public double AnswerRate { get; set; }
    public int WorkHoursCalls { get; set; }
    public int AfterHoursCalls { get; set; }
}
