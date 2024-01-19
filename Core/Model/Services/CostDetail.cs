using System;

namespace WCDS.WebFuncions.Core.Model.Services
{

    // profitCenter

    public class CostDetail
    {
        public DateTime Date { get; set; }   //date
        public string RegistrationNumber { get; set; } //registrationNumber
        public int ReportNumber { get; set; } //reportNumber
        public string Ao02Number { get; set; }
        public string RateType { get; set; }
        public decimal NumberOfUnits { get; set; }
        public string RateUnit { get; set; }
        public decimal RatePerUnit { get; set; }
        public decimal Cost { get; set; }
        public string GlAccountNumber { get; set; }
        public string ProfitCenter { get; set; }
        public string CostCenter { get; set; }
        public string FireNumber { get; set; }
        public string InternalOrder { get; set; }
        public string Fund { get; set; }


    }
}
