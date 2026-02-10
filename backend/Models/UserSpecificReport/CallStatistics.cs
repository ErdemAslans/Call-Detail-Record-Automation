namespace Cdr.Api.Models;

public class CallStatistics
{
    public int TotalCalls { get; set; }

    public int IncomingCalls { get; set; }
    
    public int AnsweredCalls { get; set; }
    
    public int MissedCalls { get; set; }

    public int RedirectedCalls { get; set; }

    public int OnBreakCalls { get; set; }

    public double AnsweredCallRate => (IncomingCalls - RedirectedCalls - OnBreakCalls) <= 0 ? 0 : Math.Round((double)AnsweredCalls / (IncomingCalls - RedirectedCalls - OnBreakCalls) * 100, 2);

    public int? TotalDuration { get; set; }
}
