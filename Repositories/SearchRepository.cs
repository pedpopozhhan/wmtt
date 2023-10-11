using System.Collections.Generic;
using System.Linq;
using WCDS.ContractUtilization.Models;

namespace WCDS.ContractUtilization.Repositories;

public interface ISearchRepository
{
    List<SearchResult> Query(SearchRequest request);
}

public class SearchRepository : ISearchRepository
{

    public List<SearchResult> Query(SearchRequest request)
    {
        var data = SampleData.GetSampleResults()
        .Where(x => x.Vendor.ToUpper().Contains(request.SearchTerm) || x.BusinessId.ToString().Contains(request.SearchTerm))
        .OrderBy(x => x.GetType().GetProperty(request.SortColumn).GetValue(x, null));

        // filter, then sort, then page
        return data.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();
    }
}