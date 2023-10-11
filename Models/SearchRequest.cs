namespace WCDS.ContractUtilization.Models;

public class SearchRequest : PagingRequest
{
    public string SearchTerm { get; set; }
    public ContractType ContractType { get; set; }
}