using System;
using System.ComponentModel.DataAnnotations;


namespace WCDS.WebFuncions.Core.Model
{
    public class UpdateInvoiceStatusRequestDto
    {
        public Guid? InvoiceId { get; set; }
        public string PaymentStatus { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
    }
}
