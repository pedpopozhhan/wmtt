using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCDS.WebFuncions.Core.Model
{
    public class CostDetailsRequestDto
    {
        public int FlightReportId { get; set; }
        public List<Guid> FlightReportCostDetailIds { get; set;}
    }
}
