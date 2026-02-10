using AutoMapper;
using Cdr.Api.Models.Response;
using System.Globalization;

namespace Cdr.Api.Models.Resolvers;

public class DayOfWeekToStringResolver : IValueResolver<IEnumerable<WeeklyAnsweredCallRate>, BarChartResponse<double>, List<string>>
{
    public List<string> Resolve(IEnumerable<WeeklyAnsweredCallRate> source, BarChartResponse<double> destination, List<string> destMember, ResolutionContext context)
    {
        // Keep the order as-is (today first, 6 days ago last)
        // Format as day name for display
        return source
            .Select(x => CultureInfo.CurrentCulture.DateTimeFormat.GetDayName((DayOfWeek)x.DayOfWeek))
            .ToList();
    }
}