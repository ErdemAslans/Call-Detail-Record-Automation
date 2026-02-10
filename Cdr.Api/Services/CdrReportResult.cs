using Cdr.Api.Models.Response;

namespace Cdr.Api.Services;

/// <summary>
/// Result object containing generated report data and metadata.
/// </summary>
public class CdrReportResult
{
    public Guid ExecutionId { get; set; }
    public string ReportType { get; set; } = string.Empty;
    public DateTime PeriodStartDate { get; set; }
    public DateTime PeriodEndDate { get; set; }
    public DateTime GeneratedAt { get; set; }
    public byte[] ExcelData { get; set; } = Array.Empty<byte>();
    public string FileName { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public int RecordsProcessed { get; set; }
    public long GenerationDurationMs { get; set; }
    public CdrReportMetricsSummary MetricsSummary { get; set; } = new();
    public string? ErrorMessage { get; set; }
    public bool IsSuccess => string.IsNullOrEmpty(ErrorMessage);
}
