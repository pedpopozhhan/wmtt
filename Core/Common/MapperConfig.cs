using AutoMapper;
using WCDS.WebFuncions.Core.Entity;
using WCDS.WebFuncions.Core.Model;
using WCDS.WebFuncions.Core.Model.Services;

namespace WCDS.WebFuncions.Core.Common
{
    public class MapperConfig
    {
        public static Mapper InitializeAutomapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Invoice, InvoiceDto>();
                //.ForMember(dest => dest.TimeReports, src => src.Ignore());

                cfg.CreateMap<InvoiceDto, Invoice>()
                .ForMember(dest => dest.TimeReports, src => src.Ignore());

                cfg.CreateMap<InvoiceTimeReport, InvoiceTimeReportDto>();
                cfg.CreateMap<InvoiceTimeReportDto, InvoiceTimeReport>()
                .ForMember(dest => dest.InvoiceDetails, src => src.Ignore());

                cfg.CreateMap<InvoiceDetail, InvoiceDetailDto>();
                cfg.CreateMap<InvoiceDetailDto, InvoiceDetail>();

                // cfg.CreateMap<CostDetailDto, CostDetail>()
                // .ForMember(dest => dest.ReportNumber, opt => opt.MapFrom(src => src.FlightReportId))
                // .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.FlightReportDate))
                // .ForMember(dest => dest.RegistrationNumber, opt => opt.MapFrom(src => src.ContractRegistrationName))
                // .ForMember(dest => dest.GlAccountNumber, opt => opt.MapFrom(src => src.Account));

            });
            var mapper = new Mapper(config);
            return mapper;
        }
    }
}
