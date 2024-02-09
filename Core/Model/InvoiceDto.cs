using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCDS.WebFuncions.Core.Entity;

namespace WCDS.WebFuncions.Core.Model
{
    public class InvoiceDto: BaseDto
    {
        public int InvoiceKey { get; set; }
        public string InvoiceId { get; set; }
        public decimal? InvoiceAmount { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public DateTime? PeriodEndDate { get; set; }
        public DateTime? InvoiceReceivedDate { get; set; }
        public string PaymentStatus { get; set; }
        public string Vendor { get; set; }
        public string AssignedTo { get; set; }
        public string ContractNumber { get; set; }
        public string Type { get; set; }
        public string CreatedBy { get; set; }
        public List<InvoiceTimeReportCostDetailDto> InvoiceTimeReportCostDetails { get; set; }
        public List<InvoiceOtherCostDetailDto> InvoiceOtherCostDetails { get; set; }
        public InvoiceServiceSheetDto InvoiceServiceSheet { get; set;}
    }
}
