using System;
using System.Collections.Generic;

namespace WCDS.WebFuncions.Core.Model
{
    public class InvoiceStatusSyncMessageDto
    {
        public DateTime TimeStamp { get; set; }
        public string Action { get; set; }
        public Guid InvoiceId { get; set; }
        public string InvoiceNumber { get; set; }
        public string PaymentStatus { get; set; }
        public List<CostDetails> Details { get; set; }

        public class CostDetails
        {
            public int FlightReportId { get; set; }
            public Guid FlightReportCostDetailsId { get; set; }
        }

    }

}
