namespace Cdr.Api.Models;

public class BarChartResponse<T> where T : struct
{
    public List<BarChartData<T>> Series { get; set; } = new List<BarChartData<T>>();

    public List<string> Labels { get; set; } = new List<string>();
}