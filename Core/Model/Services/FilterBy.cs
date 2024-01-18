namespace WCDS.WebFuncions.Core.Model.Services
{
    public class FilterBy : IFilterBy
    {
        public string ColumnName { get; set; }
        public string ColumnValue { get; set; }
        public FilterBy()
        {
            ColumnName = string.Empty;
            ColumnValue = string.Empty;
        }
    }
}