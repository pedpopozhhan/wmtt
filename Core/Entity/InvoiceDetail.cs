using System;
using System.ComponentModel.DataAnnotations;

namespace WCDS.WebFuncions.Core.Entity
{
    internal class InvoiceDetail
    {
        [Key]
        public int InvoiceDetailKey { get; set; }
        public int InvoiceTimeReportKey { get; set; }
        public int DetailKey { get; set; }
        public DateTime AuditCreationDateTime { get; set; }
        public DateTime AuditLastUpdateDateTime { get; set; }
        public InvoiceTimeReport TimeReport { get; set; }
    }
}
