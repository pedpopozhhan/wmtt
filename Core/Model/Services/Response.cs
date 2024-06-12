using System.Collections.Generic;

namespace WCDS.WebFuncions.Core.Model.Services
{
    public class Response<T>
    {
        public bool Status { get; set; }
        public string ErrorCodeId { get; set; }
        public string ErrorMessage { get; set; }
        public PaginationResponseInfo PaginationInfo { get; set; }
        public List<T> Data { get; set; }
    }
}

