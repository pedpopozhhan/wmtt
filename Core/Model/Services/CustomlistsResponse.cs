using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCDS.WebFuncions.Core.Model.Services
{
    public class CustomlistsResponse
    {
        public string[] RateUnits { get; set; }
        public string[] RateTypes { get; set; }
        public string[] PayableRateTypes { get; set; }
        public CustomlistDto[] GLAccountList { get; set; }
        public CustomlistDto[] CostCenterList { get; set; }
        public CustomlistDto[] InternalOrderList { get; set; }
        public CustomlistDto[] FundList { get; set; }
    }
}
