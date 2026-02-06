using AutoMapper;
using Cdr.Api.Models;

namespace Cdr.Api.Helpers
{
    public static class ChartHelper
    {
        public static BarChartResponse<T> ConvertToBarChartResponse<T, TList>(this IEnumerable<TList> source, IMapper mapper) 
            where T : struct
        {
            return mapper.Map<BarChartResponse<T>>(source);
        }
    }
}