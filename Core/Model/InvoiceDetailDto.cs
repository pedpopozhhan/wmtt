using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCDS.WebFuncions.Core.Model
{
    public class InvoiceDetailDto: BaseDto
    {
        public int InvoiceDetailKey { get; set; }
        public int InvoiceTimeReportKey { get; set; }
        public int DetailKey { get; set; }
        public InvoiceTimeReportDto TimeReport { get;set; }
    }
}
