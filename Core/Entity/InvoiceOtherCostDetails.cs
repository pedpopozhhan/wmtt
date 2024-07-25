using System;
using System.ComponentModel.DataAnnotations;

namespace WCDS.WebFuncions.Core.Entity
{
    internal class InvoiceOtherCostDetails : EntityBase
    {
        [Key]
        public Guid InvoiceOtherCostDetailId { get; set; }
        public Guid InvoiceId { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public string RateType { get; set; }
        public decimal NoOfUnits { get; set; }
        public string RateUnit { get; set; }
        public decimal RatePerUnit { get; set; }
        public decimal Cost { get; set; }
        public string Account { get; set; }
        public string ProfitCentre { get; set; }
        public string CostCentre { get; set; }
        public string FireNumber { get; set; }
        public string InternalOrder { get; set; }
        public string Fund { get; set; }
        public string Remarks { get; set; }
    }
}
