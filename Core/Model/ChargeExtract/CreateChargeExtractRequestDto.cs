using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCDS.WebFuncions.Core.Model.ChargeExtract
{
    public class CreateChargeExtractRequestDto
    {
        public DateTime ChargeExtractDateTime { get; set; }
        public List<string> Invoices { get; set; }
        public string ContractNumber { get; set; }
        public string RequestedBy { get; set; }
    }
}
