namespace WCDS.WebFuncions.Core.Model.Services
{
    public class FilterByRateType : FilterBy
    {
        public bool? Negotiated { get; set; }
    }

    public class FilterByRateUnit : FilterBy
    {

    }

    public class FilterByCorporateRegion : FilterBy
    {
       
    }

    public class FilterByCostRequest : FilterBy
    {
        public string ContractNumber { get; set; }
        public string Status { get; set; }
        public FilterByCostRequest(string contractNumber, string status)
        {
            ContractNumber = contractNumber;
            Status = status;
        }
    }
    public class FilterByCostDetails : FilterBy
    {
        public FlightReportIds FlightReportIds { get; set; }
        public FilterByCostDetails(int[] ids) { FlightReportIds = new FlightReportIds(ids); }
    }
}