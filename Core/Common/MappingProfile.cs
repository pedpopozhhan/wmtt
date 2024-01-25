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
            CreateMap<CostDetailDto, CostDetail>().ReverseMap();
            CreateMap<Invoice, InvoiceDto>().ReverseMap();
            CreateMap<InvoiceTimeReportCostDetails, InvoiceTimeReportCostDetailDto>().ReverseMap();
            CreateMap<InvoiceOtherCostDetails, InvoiceOtherCostDetailDto>().ReverseMap();
            CreateMap<InvoiceServiceSheet, InvoiceServiceSheetDto>().ReverseMap();
        }
    }
}