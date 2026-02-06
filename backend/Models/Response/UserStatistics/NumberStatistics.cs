namespace Cdr.Api.Models.Response.UserStatistics;

public class NumberStatistics
{
    public string? Number { get; set; }
    
    public int IncomingCallCount { get; set; }
    
    public int AnsweredCallCount { get; set; }
    
    public int MissedCallCount { get; set; }
    
    public int RedirectedCallCount { get; set; }
    
    /// <summary>
    /// Answered Call Rate = Answered / (Incoming - Redirected) × 100
    /// Consistent with CallStatistics calculation
    /// </summary>
    public double AnsweredCallRatio => (IncomingCallCount - RedirectedCallCount) > 0 
        ? Math.Round((double)AnsweredCallCount / (IncomingCallCount - RedirectedCallCount) * 100, 2) 
        : 0;
    
    public int MinDuration { get; set; }
    
    public int MaxDuration { get; set; }
    
    public double AvgDuration { get; set; }
}