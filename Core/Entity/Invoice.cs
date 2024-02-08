using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCDS.WebFuncions.Core.Model;

namespace WCDS.WebFuncions.Core.Entity
{
    internal class Invoice
    {
        [Key]
        public int InvoiceId { get; set; }
        public string InvoiceNumber { get; set; }
        public decimal? InvoiceAmount { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public DateTime? PeriodEndDate { get; set; }
        public DateTime? InvoiceReceivedDate { get; set; }
        public string PaymentStatus { get; set; }
        public string Vendor { get; set; }
        public string AssignedTo { get; set; }
        public string ContractNumber { get; set; }
        public string Type { get; set; }
        public string CreatedBy { get; set; }
        public List<InvoiceTimeReportCostDetails> InvoiceTimeReportCostDetails { get; set; }
        public List<InvoiceOtherCostDetails> InvoiceOtherCostDetails { get; set; }
        public InvoiceServiceSheet InvoiceServiceSheet { get; set; }
    }
}
