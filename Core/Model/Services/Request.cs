namespace WCDS.WebFuncions.Core.Model.Services
{
    public class Request<T> where T : IFilterBy
    {
        public string Search { get; set; }
        public string SortBy { get; set; }
        public string SortOrder { get; set; }
        public T FilterBy { get; set; }
        public PaginationInfo PaginationInfo { get; set; }

        public Request()
        {
            Search = string.Empty;
            SortBy = string.Empty;
            SortOrder = string.Empty;
            PaginationInfo = new PaginationInfo();
        }
    }
}
/*{
  "search": "",
  "sortBy": "",
  "sortOrder": "",
  "filterBy": {
    "columnName": "",
    "columnValue": ""
  },
  "paginationInfo": {
    "perPage": 2000,
    "page": 1
  }
}*/

