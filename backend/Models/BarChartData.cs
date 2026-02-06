namespace Cdr.Api.Models;

public class BarChartData<T>
{
    public required string Name { get; set; }
    
    public List<T> Data { get; set; } = new List<T>();
}