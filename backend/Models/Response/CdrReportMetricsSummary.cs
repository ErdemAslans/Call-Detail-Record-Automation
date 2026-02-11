namespace Cdr.Api.Models.Response;

/// <summary>
/// Summary metrics for CDR reports
/// </summary>
public class CdrReportMetricsSummary
{
    public int TotalIncomingCalls { get; set; }
    public int TotalAnsweredCalls { get; set; }
    public int TotalMissedCalls { get; set; }
    public int TotalOnBreakCalls { get; set; }
    public int TotalOutgoingCalls { get; set; }
    public double AnswerRate { get; set; }
    public int WorkHoursCalls { get; set; }
    public int AfterHoursCalls { get; set; }
    public List<OperatorBreakSummary> BreakSummaries { get; set; } = new();
    public List<OperatorBreakSummary> ShiftEndSummaries { get; set; } = new();
    public int TotalBreakCount { get; set; }
    public double TotalBreakDurationMinutes { get; set; }
    public int TotalShiftEndCount { get; set; }
}

/// <summary>
/// Break summary per operator for reports
/// </summary>
public class OperatorBreakSummary
{
    public string OperatorName { get; set; } = "";
    public string PhoneNumber { get; set; } = "";
    public int BreakCount { get; set; }
    public double TotalDurationMinutes { get; set; }
    public List<BreakDetail> Breaks { get; set; } = new();
}

/// <summary>
/// Individual break detail for reports
/// </summary>
public class BreakDetail
{
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public double DurationMinutes { get; set; }
    public string? Reason { get; set; }
    public string BreakType { get; set; } = "Break";
}
