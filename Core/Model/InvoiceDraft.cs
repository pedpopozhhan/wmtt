using System;

namespace WCDS.WebFuncions.Core.Model
{
    public class InvoiceDraft
    {
        public Guid? InvoiceId { get; set; }
        public string InvoiceNumber { get; set; }
        public decimal? InvoiceAmount { get; set; }
        public DateTime? InvoiceDate { get; set; }

    }
}