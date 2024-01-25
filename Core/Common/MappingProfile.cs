using AutoMapper;
using WCDS.WebFuncions.Core.Model.Services;

namespace WCDS.WebFuncions.Core.Common
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CostDetailDto, CostDetail>().ReverseMap();
        }
    }
}