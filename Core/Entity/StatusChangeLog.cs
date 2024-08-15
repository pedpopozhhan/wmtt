using System;
using System.ComponentModel.DataAnnotations;

namespace WCDS.WebFuncions.Core.Entity
{
    public class InvoiceStatusLog
    {
        [Key]
        public Guid StatusLogId { get; set; }
        public Guid InvoiceId { get; set; }
        public string PreviousStatus { get; set; }
        public string CurrentStatus { get; set; }
        public string User { get; set; }
        public DateTime Timestamp { get; set; }
        public Invoice Invoice { get; set; }
    }
}
