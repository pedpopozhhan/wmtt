using System.Collections.Generic;

namespace WCDS.ContractUtilization.Models;

public class SearchResponse : PagingResponse
{
    public List<SearchResult> SearchResults { get; set; }
}