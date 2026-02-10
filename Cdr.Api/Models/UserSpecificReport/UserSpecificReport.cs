using Cdr.Api.Models.Response.UserStatistics;

namespace Cdr.Api.Models;

public class UserSpecificReport
{

    public List<UserCallListItem> CallDetails { get; set; } = new List<UserCallListItem>();

    public List<UserCallListItem> WorkHours { get; set; } = new List<UserCallListItem>();

    public CallStatistics? WorkHoursStatistics { get; set; } 

    public List<UserCallListItem> NonWorkHours { get; set; } = new List<UserCallListItem>();

    public CallStatistics? NonWorkHoursStatistics { get; set; }

    public List<BreakTime> BreakTimes { get; set; } = new List<BreakTime>();
}