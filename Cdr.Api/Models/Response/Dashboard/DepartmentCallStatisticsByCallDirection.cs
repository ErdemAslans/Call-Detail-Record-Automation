using Cdr.Api.Models.Response;

namespace Cdr.Api.Models.Response.Dashboard;

public class DepartmentCallStatisticsByCallDirection
{
    public IEnumerable<DepartmentCallStatistics> Incoming { get; set; } = new List<DepartmentCallStatistics>();

    public IEnumerable<DepartmentCallStatistics> Outgoing { get; set; } = new List<DepartmentCallStatistics>();

    public IEnumerable<DepartmentCallStatistics> Internal { get; set; } = new List<DepartmentCallStatistics>();
} 