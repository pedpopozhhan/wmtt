using System;

namespace WCDS.WebFuncions.Core.Model.Services
{

    // profitCenter

    public class CostDetailo
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
        public string ProfitCentre { get; set; }
        public string CostCenter { get; set; }
        public string FireNumber { get; set; }
        public string InternalOrder { get; set; }
        public string Fund { get; set; }


    }
}
// export interface IDetailsTableRowData {
//   date: Date;
//   registrationNumber: string;
//   reportNumber: number;
//   aO02Number: string;
//   rateType: string;
//   numberOfUnits: number;
//   rateUnit: string;
//   ratePerUnit: number; //with $0.00
//   cost: number; //with $0.00
//   glAccountNumber: number;
//   profitCentre: string;
//   costCentre: string;
//   fireNumber: string;
//   internalOrder: string;
//   fund: number;
// }