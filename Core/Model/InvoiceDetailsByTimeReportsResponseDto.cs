using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCDS.WebFuncions.Core.Model
{
    public class InvoiceDetailsByTimeReportsResponseDto
    {
            public int flightReportId { get; set; }
            public DateTime flightReportDate { get; set; }
            public string ao02Number { get; set; }
            public string status { get; set; }
            public int contractRegistrationId { get; set; }
            public string contractRegistrationName { get; set; }
            public int contractId { get; set; }
            public string contractNumber { get; set; }
            public string vendorId { get; set; }
            public string vendorName { get; set; }
            public string financeVendorId { get; set; }
            public string rateTypeId { get; set; }
            public float noOfUnits { get; set; }
            public string rateUnitId { get; set; }
            public float cost { get; set; }
            public string account { get; set; }
            public string costCenter { get; set; }
            public string fireNumber { get; set; }
            public string internalOrder { get; set; }
            public string fund { get; set; }

    }
}
