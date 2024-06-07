namespace WCDS.WebFuncions.Core.Model.Services
{
    public class Request<T> : IRequest<T> where T : IFilterBy
    {
        public string ServiceName { get; set; }
        public string Search { get; set; }
        public string SortBy { get; set; }
        public string SortOrder { get; set; }
        public T FilterBy { get; set; }
        public PaginationInfo PaginationInfo { get; set; }

        public Request()
        {
            ServiceName = string.Empty;
            Search = string.Empty;
            SortBy = string.Empty;
            SortOrder = string.Empty;
            PaginationInfo = new PaginationInfo();
        }
    }
}
