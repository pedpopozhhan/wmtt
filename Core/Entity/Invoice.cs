using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCDS.WebFuncions.Core.Entity
{
    internal class Invoice
    {
        [Key]
        public int InvoiceKey { get; set; }
        public string InvoiceID { get; set; }
        public DateTime InvoiceDateTime { get; set; }
        public DateTime DateOnInvoice { get; set; }
        public decimal InvoiceAmount { get; set; }
        public DateTime PeriodEndDate { get; set; }
        public DateTime InvoiceReceivedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime AuditCreationDateTime { get; set; }
        public DateTime AuditLastUpdateDateTime { get; set;}
        public List<InvoiceTimeReport> TimeReports { get; set; }
    }
}
