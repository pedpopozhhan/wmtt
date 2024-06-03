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
        public Guid InvoiceId { get; set; }
        public string InvoiceNumber { get; set; }
        public decimal? InvoiceAmount { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public DateTime? PeriodEndDate { get; set; }
        public DateTime? InvoiceReceivedDate { get; set; }
        public string PaymentStatus { get; set; }
        public string InvoiceStatus { get; set; }
        public string VendorBusinessId { get; set; }
        public string VendorName { get; set; }
        public string ContractNumber { get; set; }
        public string Type { get; set; }
        public List<InvoiceTimeReportCostDetails> InvoiceTimeReportCostDetails { get; set; }
        public List<InvoiceOtherCostDetails> InvoiceOtherCostDetails { get; set; }
        public string UniqueServiceSheetName { get; set; }
        public string ServiceDescription { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedByDateTime { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedByDateTime { get; set; }
        public List<InvoiceStatusLog> InvoiceStatusLogs { get; set; }
        public Guid? ChargeExtractId { get; set; }
        public ChargeExtract ChargeExtract { get; set; }

    }
}
