using WCDS.WebFuncions.Core.Model.Services;

namespace WCDS.WebFuncions.Core.Model
{
    public class InvoiceDetailsResponse
    {
        public CostDetail[] Rows;
        public string[] RateTypes { get; set; }
    }
}