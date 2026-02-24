using Cdr.Api.Entities.Cdr;
using Cdr.Api.Models;
using Cdr.Api.Models.Pagination;
using Cdr.Api.Models.Response;
using Cdr.Api.Models.Response.Dashboard;
using Cdr.Api.Models.Response.UserStatistics;
using Common.Enums;

namespace Cdr.Api.Services.Interfaces
{
    public interface ICdrRecordsService
    {
        /// <summary>
        /// Retrieves the answered calls data for a specified report period.
        /// </summary>
        /// <param name="period">The report period for which to retrieve the data.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="BarChartResponse{Double}"/> with the answered calls data.</returns>
        Task<BarChartResponse<double>> GetAnsweredCallsAsync(ReportPeriod period);

        /// <summary>
        /// Retrieves CDR records within a specified date range.
        /// </summary>
        /// <param name="startDate">The start date of the range.</param>
        /// <param name="endDate">The end date of the range.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IEnumerable{CdrRecord}"/> with the CDR records.</returns>
        Task<IEnumerable<CdrRecord>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Retrieves the daily call report.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="DailyCallReport"/> with the daily call report data.</returns>
        Task<DailyCallReport> GetDailyCallReportAsync(DateTime? date = null);

        /// <summary>
        /// Retrieves a list of CDR records based on the specified filter.
        /// </summary>
        /// <param name="filter">The filter to apply to the CDR records list.</param>
        /// <param name="filter.PageIndex">The page index of the records list.</param>
        /// <param name="filter.PageSize">The page size of the records list.</param>
        /// <param name="filter.StartDate">The start date of the records list.</param>
        /// <param name="filter.EndDate">The end date of the records list.</param>
        /// <param name="filter.Orders">The order by fields of the records list.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="PagedResult{CdrListItem}"/> with the CDR records list.</returns>
        Task<PagedResult<CdrListItem>> GetCallReportListAsync(CdrFilter filter);

        /// <summary>
        /// Retrieves number statistics for a specified number within a date range.
        /// </summary>
        /// <param name="number">The phone number for which to retrieve statistics.</param>
        /// <param name="startDate">The start date of the range.</param>
        /// <param name="endDate">The end date of the range.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="NumberStatistics"/> with the number statistics.</returns>
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
        /// Retrieves work hours statistics based on the specified filter.
        /// </summary>
        /// <param name="filter">The filter containing the number, date range, page index, and page size.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="CallStatistics"/> with the work hours statistics.</returns>
        Task<CallStatistics> GetWorkHoursStatistics(StatisticsFilter filter);

        /// <summary>
        /// Retrieves non-work hours statistics based on the specified filter.
        /// </summary>
        /// <param name="filter">The filter containing the number, date range, page index, and page size.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="CallStatistics"/> with the non-work hours statistics.</returns>
        Task<CallStatistics> GetNonWorkHoursStatistics(StatisticsFilter filter);

        /// <summary>
        /// Retrieves break times based on the specified filter.
        /// </summary>
        /// <param name="filter">The filter containing the number, date range, page index, and page size.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of <see cref="BreakTime"/>.</returns>
        Task<List<BreakTime>> GetBreakTimes(StatisticsFilter filter);
    }
}