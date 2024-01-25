using AutoMapper;
using WCDS.WebFuncions.Core.Entity;
using WCDS.WebFuncions.Core.Model;

namespace WCDS.WebFuncions.Core.Common
{
    public class MapperConfig
    {
        public static Mapper InitializeAutomapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Invoice, InvoiceDto>();
                cfg.CreateMap<InvoiceDto, Invoice>();

                cfg.CreateMap<InvoiceTimeReportCostDetails, InvoiceTimeReportCostDetailRowDataDto>();
                cfg.CreateMap<InvoiceTimeReportCostDetailRowDataDto, InvoiceTimeReportCostDetails>();

                cfg.CreateMap<InvoiceOtherCostDetails, InvoiceOtherCostDetailRowDataDto>();
                cfg.CreateMap<InvoiceOtherCostDetailRowDataDto, InvoiceOtherCostDetails>();

                cfg.CreateMap<InvoiceServiceSheet, InvoiceServiceSheetDto>();
                cfg.CreateMap<InvoiceServiceSheetDto, InvoiceServiceSheet>();
            });
            var mapper = new Mapper(config);
            return mapper;
        }
    }
}
