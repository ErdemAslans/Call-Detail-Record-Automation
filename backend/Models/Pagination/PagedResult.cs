namespace Cdr.Api.Models.Pagination;

public class PagedResult<T>
{
    public int PageIndex { get; set; }

    public int PageSize { get; set; }

    public long TotalCount { get; set; }

    public int TotalPages { get; set; }

    public IList<T> Items { get; set; }
}