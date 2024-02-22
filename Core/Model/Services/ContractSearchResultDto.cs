using System;

namespace WCDS.WebFuncions.Core.Model.Services
{
    public class ContractSearchResultDto
    {
        // public int Index { get; set; }
        public string VendorName { get; set; }
        public string BusinessId { get; set; }
        public int ContractId { get; set; }
        public string ContractNumber { get; set; }
        public string ContractType { get; set; }
        public int NumTimeReports { get; set; }
    }
}
