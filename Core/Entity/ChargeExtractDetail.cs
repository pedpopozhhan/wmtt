using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCDS.WebFuncions.Core.Entity
{
    public class ChargeExtractDetail
    {
        [Key]
        public Guid ChargeExtractDetailId { get; set; }
        public Guid ChargeExtractId { get; set; }
        public Guid InvoiceId { get; set; }
        public DateTime AuditCreationDateTime { get; set; }
        public string AuditLastUpdatedBy { get; set; }
        public DateTime AuditLastUpdatedDateTime { get; set; }
        public ChargeExtract ChargeExtract { get; set; }
    }
}
