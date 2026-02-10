using AutoMapper;
using Cdr.Api.Models.Response;

namespace Cdr.Api.Models.Resolvers;
public class SeriesResolver<T> : IValueResolver<IEnumerable<T>, BarChartResponse<double>, List<BarChartData<double>>>
    where T : YearlyAnsweredCallRate
{
    public List<BarChartData<double>> Resolve(IEnumerable<T> source, BarChartResponse<double> destination, List<BarChartData<double>> destMember, ResolutionContext context)
    {
        return new List<BarChartData<double>>{
                new BarChartData<double>{
                    Name = "Cevaplanan Çağrı Oranı",
                    Data = source.Select(x => Math.Round(x.Percentage, 1)).ToList()
                }
            };
    }
}