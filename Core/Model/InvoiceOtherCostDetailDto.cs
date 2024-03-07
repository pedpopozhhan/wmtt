using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCDS.WebFuncions.Core.Entity;

namespace WCDS.WebFuncions.Core.Model
{
    public class InvoiceOtherCostDetailDto
    {
        public Guid InvoiceOtherCostDetailId { get; set; }
        public Guid InvoiceId { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public string RateType { get; set; }
        public decimal NoOfUnits { get; set; }
        public string RateUnit { get; set; }
        public double RatePerUnit { get; set; }
        public double Cost { get; set; }
        public string Account { get; set; }
        public string ProfitCentre { get; set; }
        public string CostCentre { get; set; }
        public string FireNumber { get; set; }
        public string InternalOrder { get; set; }
        public string Fund { get; set; }
        public string Remarks { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedByDateTime { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedByDateTime { get; set; }
    }
}
