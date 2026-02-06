using Cdr.Api.Models.Pagination;

namespace Cdr.Api.Models.Response.UserStatistics;

public class StatisticsFilter
{
    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public string? Number { get; set; }
}