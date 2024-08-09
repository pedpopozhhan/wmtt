using System;

namespace WCDS.WebFuncions.Core.Model.Services
{
    public class TimeReportCostDetailDto
    {
        public int FlightReportId { get; set; }
        public Guid FlightReportCostDetailsId { get; set; }
        public DateTime FlightReportDate { get; set; }
        public string Ao02Number { get; set; }
        public string Status { get; set; }
        public int ContractRegistrationId { get; set; }
        public string ContractRegistrationName { get; set; }
        public int ContractId { get; set; }
        public string ContractNumber { get; set; }
        public string VendorId { get; set; }
        public string VendorName { get; set; }
        public string FinanceVendorId { get; set; }
        public string RateTypeId { get; set; }
        public decimal NoOfUnits { get; set; }
        public string RateUnitId { get; set; }
        public decimal Cost { get; set; }
        public string Account { get; set; }
        public string CostCenter { get; set; }
        public string FireNumber { get; set; }
        public string FireYear { get; set; }
        public string InternalOrder { get; set; }
        public string Fund { get; set; }
        public decimal RatePerUnit { get; set; }
        public Guid InvoiceID { get; set; }
    }
}