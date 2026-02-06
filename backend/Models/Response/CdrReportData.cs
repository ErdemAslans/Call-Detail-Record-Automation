namespace Cdr.Api.Models.Response;

/// <summary>
/// Detailed report data structure containing all 15 metric sections.
/// Used for Excel generation and API responses.
/// </summary>
public class CdrReportData
{
    /// <summary>
    /// Report metadata
    /// </summary>
    public ReportMetadata Metadata { get; set; } = new();

    /// <summary>
    /// Summary/Overview sheet data
    /// </summary>
    public ReportSummary Summary { get; set; } = new();

    /// <summary>
    /// Department-level statistics
    /// </summary>
    public List<DepartmentMetrics> DepartmentStatistics { get; set; } = new();

    /// <summary>
    /// Incoming calls details
    /// </summary>
    public CallDirectionMetrics IncomingCalls { get; set; } = new();

    /// <summary>
    /// Outgoing calls details
    /// </summary>
    public CallDirectionMetrics OutgoingCalls { get; set; } = new();

    /// <summary>
    /// Internal calls details
    /// </summary>
    public CallDirectionMetrics InternalCalls { get; set; } = new();

    /// <summary>
    /// Work hours traffic (07:45-16:45 weekdays)
    /// </summary>
    public WorkHoursMetrics WorkHoursTraffic { get; set; } = new();

    /// <summary>
    /// After-hours traffic (outside work hours + weekends + holidays)
    /// </summary>
    public WorkHoursMetrics AfterHoursTraffic { get; set; } = new();

    /// <summary>
    /// Operator break records
    /// </summary>
    public List<BreakRecord> BreakRecords { get; set; } = new();

    /// <summary>
    /// Call redirection audit trail
    /// </summary>
    public List<RedirectionRecord> Redirections { get; set; } = new();

    /// <summary>
    /// Dropped/failed calls
    /// </summary>
    public List<DroppedCallRecord> DroppedCalls { get; set; } = new();
}
