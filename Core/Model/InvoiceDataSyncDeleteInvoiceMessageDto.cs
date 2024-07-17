using System;
using System.Collections.Generic;
using WCDS.WebFuncions.Core.Entity;

namespace WCDS.WebFuncions.Core.Model
{
    public class InvoiceDataSyncDeleteInvoiceMessageDto
    {
        public DateTime TimeStamp { get; set; }
        public string Action { get; set; }
        public InvoiceDataSyncDeleteInvoiceMessageDetailDto Tables { get; set; }

    }
    public class InvoiceDataSyncDeleteInvoiceMessageDetailDto
    {
        public InvoiceDataSyncDeleteInvoiceMessageDetailInvoiceDto Invoice { get; set; }
    }
    public class InvoiceDataSyncDeleteInvoiceMessageDetailInvoiceDto
    {
        public Guid InvoiceId { get; set; }

        public InvoiceDataSyncDeleteInvoiceMessageDetailCostDto Tables { get; set; }
    }
    public class InvoiceDataSyncDeleteInvoiceMessageDetailCostDto
    {
        public List<Guid> InvoiceTimeReportCostDetails { get; set; }
        public List<Guid> InvoiceOtherCostDetails { get; set; }
        public List<Guid> InvoiceTimeReports { get; set; }

    }


}
