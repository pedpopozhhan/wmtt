using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCDS.WebFuncions.Core.Entity;
using WCDS.WebFuncions.Core.Model.ChargeExtract;

namespace WCDS.WebFuncions.Core.Model
{
    public class InvoiceDto : BaseDto
    {
        public Guid? InvoiceId { get; set; }
        public string InvoiceNumber { get; set; }
        public decimal? InvoiceAmount { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public DateTime? PeriodEndDate { get; set; }
        public DateTime? InvoiceReceivedDate { get; set; }
        public string PaymentStatus { get; set; }
        public string VendorBusinessId { get; set; }
        public string VendorName { get; set; }
        public string ContractNumber { get; set; }
        public string Type { get; set; }
        public List<InvoiceTimeReportCostDetailDto> InvoiceTimeReportCostDetails { get; set; }
        public List<InvoiceOtherCostDetailDto> InvoiceOtherCostDetails { get; set; }
        public string UniqueServiceSheetName { get; set; }
        public string ServiceDescription { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedByDateTime { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedByDateTime { get; set; }
        public Guid? ChargeExtractId { get; set; }
        public ChargeExtractDto ChargeExtract { get; set; }
        public DateTime? DocumentDate
        {
            get
            {
                if (string.IsNullOrEmpty(ChargeExtractId.ToString()))
                {
                    return null;
                }
                else
                {
                    return ChargeExtract?.ChargeExtractDateTime;
                }
            }
        }
    }
}
