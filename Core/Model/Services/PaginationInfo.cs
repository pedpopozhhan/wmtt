namespace WCDS.WebFuncions.Core.Model.Services
{
    public class PaginationInfo
    {
        public int PerPage { get; set; }
        public int Page { get; set; }

        public PaginationInfo()
        {
            Page = 1;
            PerPage = 10000;
        }
    }
    public class PaginationResponseInfo : PaginationInfo
    {
        public int TotalPages { get; set; }
        public int Total { get; set; }
    }
}
