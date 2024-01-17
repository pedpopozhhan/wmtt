namespace WCDS.WebFuncions.Core.Model.Services
{
    public class PaginationInfo
    {
        public int PerPage { get; set; }
        public int Page { get; set; }

        public PaginationInfo()
        {
            Page = 1;
            PerPage = 1000;
        }
    }
    public class PaginationResponseInfo : PaginationInfo
    {
        public int TotalPages { get; set; }
        public int Total { get; set; }
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
