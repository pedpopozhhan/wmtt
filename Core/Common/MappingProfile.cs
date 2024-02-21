using AutoMapper;
using WCDS.WebFuncions.Core.Entity;
using WCDS.WebFuncions.Core.Model;
using WCDS.WebFuncions.Core.Model.Services;

namespace WCDS.WebFuncions.Core.Common
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<TimeReportCostDetailDto, TimeReportCostDetail>().ForMember(dest => dest.TimeReportCostDetailReferenceId, opt => opt.MapFrom(src => src.FlyingHoursId)).ReverseMap();
            CreateMap<Invoice, InvoiceDto>().ReverseMap();
            CreateMap<InvoiceTimeReportCostDetails, InvoiceTimeReportCostDetailDto>().ForMember(dest => dest.FlightReportId, opt => opt.MapFrom(src => src.ReportNumber)).ReverseMap();
            CreateMap<InvoiceOtherCostDetails, InvoiceOtherCostDetailDto>().ReverseMap();
            CreateMap<InvoiceServiceSheet, InvoiceServiceSheetDto>().ReverseMap();
        }
    }
}