using System.Globalization;
using AutoMapper;

namespace Cdr.Api.Models.Response;

public class MonthOfYearToStringResolver : IValueResolver<IEnumerable<MonthlyAnsweredCallRate>, BarChartResponse<double>, List<string>>
{
    public List<string> Resolve(IEnumerable<MonthlyAnsweredCallRate> source, BarChartResponse<double> destination, List<string> destMember, ResolutionContext context)
    {
        return source
            .Select(x => x.Month)
            .OrderBy(x => x)
            .Select(CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName)
            .ToList();
    }
}