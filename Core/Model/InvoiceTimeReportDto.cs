using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCDS.WebFuncions.Core.Model
{
    public class InvoiceTimeReportDto: BaseDto
    {
        public int InvoiceTimeReportKey { get; set; }
        public int InvoiceKey { get; set; }
        public int TimeReportNumber { get; set; }
        public List<InvoiceDetailDto> InvoiceDetails { get; set; }
    }
}
