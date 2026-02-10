namespace Cdr.Api.Models.Response.UserStatistics;

public class NumberStatistics
{
    public string? Number { get; set; }
    
    public int IncomingCallCount { get; set; }
    
    public int AnsweredCallCount { get; set; }
    
    public int MissedCallCount { get; set; }
    
    public int RedirectedCallCount { get; set; }

    public int OnBreakCallCount { get; set; }

    /// <summary>
    /// Answered Call Rate = Answered / (Incoming - Redirected - OnBreak) × 100
    /// Calls during break are excluded from answer rate calculation
    /// </summary>
    public double AnsweredCallRatio => (IncomingCallCount - RedirectedCallCount - OnBreakCallCount) > 0
        ? Math.Round((double)AnsweredCallCount / (IncomingCallCount - RedirectedCallCount - OnBreakCallCount) * 100, 2)
        : 0;
    
    public int MinDuration { get; set; }
    
    public int MaxDuration { get; set; }
    
    public double AvgDuration { get; set; }
}