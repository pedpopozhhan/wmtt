using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCDS.WebFuncions.Core.Model
{
    public class InvoiceDto: BaseDto
    {
        public int InvoiceKey { get; set; }
        public string? InvoiceID { get; set; }
        public DateTime? InvoiceDateTime { get; set; }
        public DateTime? DateOnInvoice { get; set; }
        public decimal? InvoiceAmount { get; set; }
        public DateTime? PeriodEndDate { get; set; }
        public DateTime InvoiceReceivedDate { get; set; }
        public string? CreatedBy { get; set; }
        public List<InvoiceTimeReportDto> TimeReports { get; set;}
    }
}
