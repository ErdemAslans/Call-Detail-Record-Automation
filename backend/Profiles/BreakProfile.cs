using AutoMapper;
using Cdr.Api.Helpers;
using Cdr.Api.Models.Entities;
using Cdr.Api.Models.Response;

namespace Cdr.Api.Profiles
{
    public class BreakProfile : Profile
    {
        public BreakProfile()
        {
            CreateMap<Break, BreakResponseModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => TurkeyTimeProvider.ConvertFromUtc(src.StartTime)))
                .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.EndTime.HasValue ? TurkeyTimeProvider.ConvertFromUtc(src.EndTime.Value) : (DateTime?)null))
                .ForMember(dest => dest.PlannedEndTime, opt => opt.MapFrom(src => TurkeyTimeProvider.ConvertFromUtc(src.PlannedEndTime)));
        }
    }
}
