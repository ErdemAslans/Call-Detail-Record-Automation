using AutoMapper;
using Cdr.Api.Models.Entities;
using Cdr.Api.Models.Response;

namespace Cdr.Api.Profiles
{
    public class BreakProfile : Profile
    {
        public BreakProfile()
        {
            CreateMap<Break, BreakResponseModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()));
        }
    }
}
