namespace Cdr.Api.Models.Response;

public class DailyCallReport
{
    public int TotalCalls { get; set; }

    public int AnsweredCalls { get; set; }

    public int MissedCalls { get; set; }

    public int OnBreakCalls { get; set; }

    public int TotalDuration { get; set; }

    public int ClosedAtIVR { get; set; }

    public int MissedCallbackCalls { get; set; }

    public double AverageDuration => TotalCalls == 0 ? 0 : Math.Round((double)TotalDuration / TotalCalls, 2);

    public double AnsweredCallRate => (TotalCalls - OnBreakCalls) == 0 ? 0 : Math.Round((double)AnsweredCalls / (TotalCalls - OnBreakCalls) * 100, 2);
}