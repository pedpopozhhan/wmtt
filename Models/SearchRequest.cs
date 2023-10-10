namespace WCDS.ContractUtilization.Models;

public class SearchRequest : PagingRequest
{
    public string SearchTerm { get; set; }
}