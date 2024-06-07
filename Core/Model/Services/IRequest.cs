using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCDS.WebFuncions.Core.Model.Services
{
    public interface IRequest<T> where T : IFilterBy 
    {
        public string ServiceName { get; set; }
        public string Search { get; set; }
        public string SortBy { get; set; }
        public string SortOrder { get; set; }
        public T FilterBy { get; set; }
        public PaginationInfo PaginationInfo { get; set; }
    }
}
