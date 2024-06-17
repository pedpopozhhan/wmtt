using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCDS.WebFuncions.Core.Model.FinanceDocument
{
    public class FinanceDocumentRequestDto
    {
        public string InvoiceNumber { get; set; }
        public decimal InvoiceAmount { get; set; }
        public string VendorBusinessId { get; set; }
    }
}
