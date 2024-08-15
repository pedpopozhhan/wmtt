using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCDS.WebFuncions.Core.Entity
{
    public class InvoiceTimeReports
    {
        [Key]
        public Guid InvoiceTimeReportId { get; set; }
        public Guid InvoiceId { get; set; }
        public int FlightReportId { get; set; }
        public DateTime AuditCreationDateTime { get; set; }
        public string AuditLastUpdatedBy { get; set; }
        public DateTime AuditLastUpdatedDateTime { get; set; }
        public Invoice Invoice { get; set; }
    }
}

