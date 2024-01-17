namespace WCDS.WebFuncions.Core.Model.Services
{
    public class FilterByRateType : FilterBy
    {
        public bool Negotiated { get; set; }
        public FilterByRateType()
        {
            Negotiated = true;
        }
    }

    public class FilterByRateUnit : FilterBy
    {

    }

    public class FilterByCostDetails : FilterBy
    {
        public FlightReportIds FlightReportIds { get; set; }
        public FilterByCostDetails(int[] ids) { FlightReportIds = new FlightReportIds(ids); }
    }
}