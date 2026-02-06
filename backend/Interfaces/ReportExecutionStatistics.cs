namespace Cdr.Api.Interfaces;

/// <summary>
/// Statistics for report execution monitoring.
/// </summary>
public class ReportExecutionStatistics
{
    public int TotalExecutions { get; set; }
    public int SuccessfulExecutions { get; set; }
    public int FailedExecutions { get; set; }
    public double SuccessRate => TotalExecutions > 0 ? (double)SuccessfulExecutions / TotalExecutions * 100 : 0;
    public long AverageGenerationTimeMs { get; set; }
    public long MaxGenerationTimeMs { get; set; }
    public long TotalRecordsProcessed { get; set; }
    public Dictionary<string, int> ExecutionsByReportType { get; set; } = new();
    public Dictionary<string, int> ExecutionsByStatus { get; set; } = new();
}
