namespace Cdr.Api.Models.Response;

public class WeeklyAnsweredCallRate: YearlyAnsweredCallRate
{
    public int Month { get; set; }
    public int DayOfWeek { get; set; }
    
    /// <summary>
    /// The actual date this record represents (in Turkey timezone).
    /// Used for ordering and display purposes.
    /// </summary>
    public DateTime Date { get; set; }
}