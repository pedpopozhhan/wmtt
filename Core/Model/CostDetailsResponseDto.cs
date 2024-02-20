using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCDS.WebFuncions.Core.Model.Services;

namespace WCDS.WebFuncions.Core.Model
{
    public class CostDetailsResponseDto
    {
        public CostDetailsResponseDto()
        {
            CostDetails = new List<CostDetailsResult>();
        }
        public List<CostDetailsResult> CostDetails;
        public class CostDetailsResult
        {
            public Guid FlightReportCostDetailId { get; set; }
            public int FlightReportId { get; set; }
            public string InvoiceId { get; set; }
            public string PaymentStatus { get; set; }
            public string RedirectionURL { get; set; }
        }
    }
}
