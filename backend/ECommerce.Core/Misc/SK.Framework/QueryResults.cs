using System.Collections.Generic;

namespace SK.Framework;

public class QueryResults<T> where T : class
{
    public List<T> Items { get; private set; }

    public int TotalResults { get; private set; }

    public QueryResults(List<T> res, int totalResults)
    {
        Items = res;
        TotalResults = totalResults;
    }

    public IQuerySetPaging<T> GetPagingInfo(int page, int pageSize)
        => new QuerySetPaging<T>(Items, page, pageSize, TotalResults);
}
