using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCDS.WebFuncions.Core.Model.ChargeExtract
{
    public class ChargeExtractDetailDto
    {
        public Guid ChargeExtractDetailId { get; set; }
        public Guid ChargeExtractId { get; set; }
        public Guid InvoiceId { get; set; }
        public DateTime AuditCreationDateTime { get; set; }
        public string AuditLastUpdatedBy { get; set; }
        public DateTime AuditLastUpdatedDateTime { get; set; }
    }
}
