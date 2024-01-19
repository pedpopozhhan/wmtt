using AutoMapper;
using WCDS.WebFuncions.Core.Model.Services;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CostDetailDto, CostDetail>()
                .ForMember(dest => dest.ReportNumber, opt => opt.MapFrom(src => src.FlightReportId))
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.FlightReportDate))
                .ForMember(dest => dest.RegistrationNumber, opt => opt.MapFrom(src => src.ContractRegistrationName))
                .ForMember(dest => dest.GlAccountNumber, opt => opt.MapFrom(src => src.Account));
    }
}