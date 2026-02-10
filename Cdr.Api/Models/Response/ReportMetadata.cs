namespace Cdr.Api.Models.Response;

/// <summary>
/// Report metadata
/// </summary>
public class ReportMetadata
{
    public DateTime GeneratedAt { get; set; }
    public string ReportType { get; set; } = string.Empty;
    public DateTime PeriodStartDate { get; set; }
    public DateTime PeriodEndDate { get; set; }
}
