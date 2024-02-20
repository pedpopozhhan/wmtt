using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCDS.WebFuncions.Core.Model.Services;

namespace WCDS.WebFuncions.Core.Model
{
    public class ProcessedCostDetailsResponseDto
    {
        public ProcessedCostDetailsResponseDto()
        {
            ProcessedCostDetails = new List<ProcessedCostDetailsResult>();
        }
        public List<ProcessedCostDetailsResult> ProcessedCostDetails;
        public class ProcessedCostDetailsResult
        {
            public Guid FlightReportCostDetailId { get; set; }
            public int FlightReportId { get; set; }
            public string InvoiceId { get; set; }
            public string PaymentStatus { get; set; }
            public string RedirectionURL { get; set; }
        }
    }
}
