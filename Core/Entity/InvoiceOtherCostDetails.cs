using System;
using System.ComponentModel.DataAnnotations;

namespace WCDS.WebFuncions.Core.Entity
{
    internal class InvoiceOtherCostDetails
    {
        [Key]
        public int InvoiceOtherCostDetailId { get; set; }
        public int InvoiceId { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public string RateType { get; set; }
        public string Unit { get; set; }
        public double RatePerUnit { get; set; }
        public int NumberOfUnits { get; set; }
        public double Cost { get; set; }
        public int? GlAccountNumber { get; set; }
        public string ProfitCentre { get; set; }
        public string CostCentre { get; set; }
        public string FireNumber { get; set; }
        public string InternalOrder { get; set; }
        public string Fund { get; set; }
        public string Remarks { get; set; }
        public Invoice Invoice { get; set; }
    }
}
