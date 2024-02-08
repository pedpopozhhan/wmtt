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
        public string[] GLAccountList { get; set; }
        public string[] CostCenterList { get; set; }
        public string[] InternalOrderList { get; set; }
        public string[] FundList { get; set; }
    }
}
