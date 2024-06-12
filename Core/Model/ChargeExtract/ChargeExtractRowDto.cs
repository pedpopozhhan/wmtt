using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCDS.WebFuncions.Core.Model.ChargeExtract
{
    public class ChargeExtractRowDto
    {
        public string InvoiceNumber { get; set; }
        public string CostCenter { get; set; }
        public string InternalOrder { get; set; }
        public string Fund { get; set; }
        public decimal InvoiceAmount { get; set; }
    }
}
