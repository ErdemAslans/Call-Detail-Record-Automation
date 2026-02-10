using Cdr.Api.Models.Pagination;

namespace Cdr.Api.Models;

public class CdrFilter: PagedRequest
{
    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public CallDirection? CallDirection { get; set; }

    public string? User { get; set; }
}