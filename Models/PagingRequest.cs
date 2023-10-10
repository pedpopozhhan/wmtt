namespace WCDS.ContractUtilization.Models;

public class PagingRequest
{
    // public string Type{get;set;}  not known what type it is yet
    public string SortColumn { get; set; }
    public bool Ascending { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}
