namespace Cdr.Api.Models.Response;

public class MonthlyAnsweredCallRate : YearlyAnsweredCallRate
{
    public int Month { get; set; }
}