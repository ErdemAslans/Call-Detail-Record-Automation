namespace Cdr.Api.Models.Response;

public class YearlyAnsweredCallRate
{
    public int Year { get; set; }

    public string? Quarter { get; set; }

    public int TotalRecords { get; set; }

    public int ConnectAndDuration { get; set; }

    public double Percentage { get; set; }

    public string PercentageString => $"{Percentage:0.00}%";
}