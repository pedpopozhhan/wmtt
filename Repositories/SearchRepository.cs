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

    // https://goa-dio.atlassian.net/wiki/spaces/WDS/pages/2727772233/Get+Vendor+Name+by+StakeholderId
    // aviation api for vendor, contractorid
    public List<SearchResult> Query(SearchRequest request)
    {
        var data = SampleData.GetSampleResults()
        .Where(x => x.Vendor.ToUpper().Contains(request.SearchTerm) || x.BusinessId.ToString().Contains(request.SearchTerm))
        .Where(x => request.ContractType == ContractType.Both || x.Type == request.ContractType);
        // .OrderBy(x => x.GetType().GetProperty(request.SortColumn).GetValue(x, null));

        // filter, then sort, then page
        return data.ToList();//.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();
    }
}