using System;

namespace WCDS.WebFuncions.Core.Model.Services
{
    public class TimeReportCostDto
    {
        public string FlightReportDashboardId { get; set; }
        public int FlightReportId { get; set; }
        public string CorporateRegionId { get; set; }
        public string CorporateRegionName { get; set; }
        public int ContractRegistrationId { get; set; }
        public string ContractRegistrationName { get; set; }
        public string VendorId { get; set; }
        public string VendorName { get; set; }
        public int ContractId { get; set; }
        public string ContractNumber { get; set; }
        public string FinanceVendorId { get; set; }
        public DateTime FlightReportDate { get; set; }
        public string Ao02Number { get; set; }
        public string Status { get; set; }
        public decimal TotalCost { get; set; }
        public bool IsFlagged { get; set; }
    }
}

