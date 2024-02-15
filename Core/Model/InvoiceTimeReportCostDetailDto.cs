

using System;
using System.ComponentModel.DataAnnotations;
using WCDS.WebFuncions.Core.Entity;

namespace WCDS.WebFuncions.Core.Model
{
    public class InvoiceTimeReportCostDetailDto
    {
        public int InvoiceTimeReportCostDetailId { get; set; }
        public Guid TimeReportCostDetailReferenceId { get; set; }
        public DateTime Date { get; set; }
        public string RegistrationNumber { get; set; }
        public int ReportNumber { get; set; }
        public string AO02Number { get; set; }
        public string RateType { get; set; }
        public int NumberOfUnits { get; set; }
        public string RateUnit { get; set; }
        public double RatePerUnit { get; set; }
        public double Cost { get; set; }
        public string GlAcct { get; set; }
        public string ProfitCentre { get; set; }
        public string CostCentre { get; set; }
        public string FireNumber { get; set; }
        public string InternalOrder { get; set; }
        public string Fund { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedByDateTime { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedByDateTime { get; set; }
    }
}
