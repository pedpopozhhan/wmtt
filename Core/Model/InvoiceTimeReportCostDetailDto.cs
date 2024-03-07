

using System;
using System.ComponentModel.DataAnnotations;
using WCDS.WebFuncions.Core.Entity;

namespace WCDS.WebFuncions.Core.Model
{
    public class InvoiceTimeReportCostDetailDto
    {
        public Guid FlightReportCostDetailsId { get; set; }
        public Guid InvoiceId { get; set; }
        public DateTime FlightReportDate { get; set; }
        public string ContractRegistrationName { get; set; }
        public int FlightReportId { get; set; }
        public string Ao02Number { get; set; }
        public string RateType { get; set; }
        public decimal NoOfUnits { get; set; }
        public string RateUnit { get; set; }
        public decimal RatePerUnit { get; set; }
        public decimal Cost { get; set; }
        public string Account { get; set; }
        public string ProfitCenter { get; set; }
        public string CostCenter { get; set; }
        public string FireNumber { get; set; }
        public string InternalOrder { get; set; }
        public string Fund { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedByDateTime { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedByDateTime { get; set; }
    }
}
