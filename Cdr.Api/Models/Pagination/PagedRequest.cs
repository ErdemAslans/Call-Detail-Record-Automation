using System.ComponentModel.DataAnnotations;

namespace Cdr.Api.Models.Pagination;

public class PagedRequest
{
    public PagedRequest()
    {
        Orders = new List<PagedRequestOrder>();
    }

    public List<PagedRequestOrder> Orders { get; set; }

    [Range(0, int.MaxValue)]
    public int PageIndex { get; set; }

    [Range(1, int.MaxValue)]
    public int PageSize { get; set; }
}