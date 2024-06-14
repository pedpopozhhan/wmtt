using AutoMapper;
using WCDS.WebFuncions.Core.Entity;
using WCDS.WebFuncions.Core.Model;
using WCDS.WebFuncions.Core.Model.ChargeExtract;
using WCDS.WebFuncions.Core.Model.Services;

namespace WCDS.WebFuncions.Core.Common
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Invoice, InvoiceDto>()
            .ForMember(dest => dest.AuditCreationDateTime, opt => opt.Ignore())
            .ForMember(dest => dest.AuditLastUpdateDateTime, opt => opt.Ignore())
            .ForMember(dest => dest.InvoiceTimeReportCostDetails, opt => opt.MapFrom(src => src.InvoiceTimeReportCostDetails))
            .ReverseMap();
            CreateMap<Invoice, InvoiceDataSyncMessageDetailInvoiceDto>()
            .ForMember(i => i.Tables, opt => opt.Ignore())
            .ForMember(i => i.InvoiceId, dest => dest.MapFrom(src => src.InvoiceId))
            .ForMember(i => i.InvoiceNumber, dest => dest.MapFrom(src => src.InvoiceNumber))
            .ForMember(i => i.VendorName, dest => dest.MapFrom(src => src.VendorName))
            .ForMember(i => i.VendorBusinessId, dest => dest.MapFrom(src => src.VendorBusinessId))
            .ForMember(i => i.ContractNumber, dest => dest.MapFrom(src => src.ContractNumber))
            .ForMember(i => i.Type, dest => dest.MapFrom(src => src.Type))
            .ForMember(i => i.InvoiceAmount, dest => dest.MapFrom(src => src.InvoiceAmount))
            .ForMember(i => i.InvoiceDate, dest => dest.MapFrom(src => src.InvoiceDate))
            .ForMember(i => i.InvoiceReceivedDate, dest => dest.MapFrom(src => src.InvoiceReceivedDate))
            .ForMember(i => i.PeriodEndDate, dest => dest.MapFrom(src => src.PeriodEndDate))
            .ForMember(i => i.PaymentStatus, dest => dest.MapFrom(src => src.PaymentStatus))
            .ForMember(i => i.UniqueServiceSheetName, dest => dest.MapFrom(src => src.UniqueServiceSheetName))
            .ForMember(i => i.ServiceDescription, dest => dest.MapFrom(src => src.ServiceDescription))
            .ForMember(i => i.CreatedBy, dest => dest.MapFrom(src => src.CreatedBy))
            .ForMember(i => i.CreatedByDateTime, dest => dest.MapFrom(src => src.CreatedByDateTime))
            .ForMember(i => i.UpdatedBy, dest => dest.MapFrom(src => src.UpdatedBy))
            .ForMember(i => i.UpdatedByDateTime, dest => dest.MapFrom(src => src.UpdatedByDateTime)).ReverseMap();


            CreateMap<TimeReportCostDetailDto, TimeReportCostDetail>()
            .ForMember(x => x.RateType, opt => opt.Ignore())
            .ForMember(x => x.RateUnit, opt => opt.Ignore()).ReverseMap();
            CreateMap<InvoiceTimeReportCostDetails, InvoiceTimeReportCostDetailDto>().ReverseMap();
            CreateMap<InvoiceOtherCostDetails, InvoiceOtherCostDetailDto>().ReverseMap();
            CreateMap<ChargeExtract, ChargeExtractDto>()
            .ForMember(x => x.ExtendedExtract, opt => opt.Ignore())
            .ForMember(x => x.ExtractFile, opt => opt.Ignore())
            .ForMember(x => x.ExtractFiles, opt => opt.Ignore());
            CreateMap<ChargeExtractDto, ChargeExtract>()
            .ForMember(x => x.Invoice, opt => opt.Ignore());

            CreateMap<ChargeExtractDetail, ChargeExtractDetailDto>().ReverseMap();
            CreateMap<ChargeExtractViewLog, ChargeExtractViewLogDto>().ReverseMap();
            CreateMap<Invoice, InvoiceRequestDto>().ForMember(x => x.FlightReportIds, opt => opt.Ignore()).ReverseMap();
            CreateMap<InvoiceTimeReports, InvoiceTimeReportsDto>().ReverseMap();


        }
    }
}