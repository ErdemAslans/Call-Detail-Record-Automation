using Cdr.Api.Entities.Cdr;
using Cdr.Api.Models;
using Cdr.Api.Models.Pagination;
using Cdr.Api.Models.Response;
using Cdr.Api.Models.Response.Dashboard;
using Cdr.Api.Models.Response.UserStatistics;

namespace Cdr.Api.Interfaces;

public interface ICdrRecordsRepository : IReadonlyMongoRepository<CdrRecord>
{
    Task<IEnumerable<CdrRecord>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

    Task<IEnumerable<WeeklyAnsweredCallRate>> GetWeeklyAnsweredCallsAsync(DateTime startDate, DateTime endDate);

    Task<IEnumerable<MonthlyAnsweredCallRate>> GetMonthlyAnsweredCallsAsync(DateTime startDate, DateTime endDate);

    Task<IEnumerable<YearlyAnsweredCallRate>> GetYearlyAnsweredCallsAsync(DateTime startDate, DateTime endDate);

    Task<DailyCallReport> GetDailyCallReportAsync(DateTime date);

    Task<PagedResult<CdrListItem>> GetCallReportListAsync(CdrFilter filter);

    /// <summary>
    /// Retrieves the number statistics for a given phone number within a specified date range.
    /// </summary>
    /// <param name="number">The phone number for which to retrieve statistics.</param>
    /// <param name="startDate">The start date of the date range.</param>
    /// <param name="endDate">The end date of the date range.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the number statistics.</returns>
    Task<NumberStatistics> GetNumberStatisticsByNumberAsync(string number, DateTime startDate, DateTime endDate);

    /// <summary>
    /// Retrieves the last calls for a specified number within a date range.
    /// </summary>
    /// <param name="filter">The filter containing the number, date range, page index, and page size.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="PagedResult{UserCallListItem}"/> with the last calls.</returns>
    Task<PagedResult<UserCallListItem>> GetUserLastCallsAsync(UserStatisticsFilter filter);

    /// <summary>
    /// Retrieves call statistics for each department within the specified date range.
    /// </summary>
    /// <param name="startDate">The start date of the date range.</param>
    /// <param name="endDate">The end date of the date range.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable of department call statistics.</returns>
    Task<DepartmentCallStatisticsByCallDirection> GetDepartmentCallStatisticsAsync(DateTime startDate, DateTime endDate);

    /// <summary>
    /// Retrieves user-specific call report based on the specified filter.
    /// </summary>
    /// <param name="filter">The filter containing the number, date range, page index, and page size.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="UserSpecificReport"/> with the user-specific call report.</returns>
    Task<UserSpecificReport> GetUserCalls(StatisticsFilter filter);

    /// <summary>
    /// Counts incoming calls classified as work-hours vs after-hours for a date range.
    /// </summary>
    Task<(int WorkHoursCalls, int AfterHoursCalls)> GetWorkHoursCallCountsAsync(
        DateTime startDate, DateTime endDate, List<DateOnly> holidayDates);
}