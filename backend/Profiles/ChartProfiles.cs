using AutoMapper;
using Cdr.Api.Models.Resolvers;
using Cdr.Api.Models.Response;

namespace Cdr.Api.Models.Profiles;

public class ChartProfiles : Profile
{
    public ChartProfiles()
    {
        CreateMap<WeeklyAnsweredCallRate, BarChartData<double>>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => "Cevaplanan Çağrı Oranı"))
            .ForMember(dest => dest.Data, opt => opt.MapFrom(src => src.Percentage));

        CreateMap<IEnumerable<WeeklyAnsweredCallRate>, BarChartResponse<double>>()
            .ForMember(dest => dest.Series, opt => opt.MapFrom<SeriesResolver<WeeklyAnsweredCallRate>>())
            .ForMember(dest => dest.Labels, opt => opt.MapFrom<DayOfWeekToStringResolver>());

        CreateMap<IEnumerable<MonthlyAnsweredCallRate>, BarChartResponse<double>>()
            .ForMember(dest => dest.Series, opt => opt.MapFrom<SeriesResolver<MonthlyAnsweredCallRate>>())
            .ForMember(dest => dest.Labels, opt => opt.MapFrom<MonthOfYearToStringResolver>());

        CreateMap<IEnumerable<YearlyAnsweredCallRate>, BarChartResponse<double>>()
            .ForMember(dest => dest.Series, opt => opt.MapFrom<SeriesResolver<YearlyAnsweredCallRate>>())
            .ForMember(dest => dest.Labels, opt => opt.MapFrom(src => src.Select(x => x.Quarter).OrderBy(x => x).ToList()));
    }
}