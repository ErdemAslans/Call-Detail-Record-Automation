namespace Cdr.Api.Interfaces;

/// <summary>
/// Statistics for email delivery operations.
/// </summary>
public class EmailDeliveryStatistics
{
    public int TotalSent { get; set; }
    public int TotalSuccessful { get; set; }
    public int TotalFailed { get; set; }
    public int TotalPending { get; set; }
    public double SuccessRate => TotalSent > 0 ? (double)TotalSuccessful / TotalSent * 100 : 0;
    public Dictionary<string, int> FailureReasons { get; set; } = new();
}
