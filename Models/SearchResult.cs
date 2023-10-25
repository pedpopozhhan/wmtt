using System;
using System.Collections.Generic;

namespace WCDS.ContractUtilization.Models;
public class SearchResult
{
    public String Vendor { get; set; }
    public int BusinessId { get; set; }
    public int ContractId { get; set; }
    public ContractType Type { get; set; }
    public int NumTimeReports { get; set; }
}


public static class SampleData
{
    public static List<SearchResult> GetSampleResults()
    {
        var results = new List<SearchResult>();

        for (int i = 1; i <= 50; i++)
        {
            results.Add(new SearchResult
            {
                Vendor = $"Vendor{i}",
                BusinessId = 200 + i,
                ContractId = 100 + i,
                Type = (i % 2 == 0) ? ContractType.Casual : ContractType.LongTerm,
                NumTimeReports = i
            });
        }

        return results;
    }
}