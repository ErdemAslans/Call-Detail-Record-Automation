using AutoMapper;
using Cdr.Api.Entities.Cdr;
using Cdr.Api.Helpers;
using Cdr.Api.Interfaces;
using Cdr.Api.Models;
using Cdr.Api.Models.Pagination;
using Cdr.Api.Models.Response;
using Cdr.Api.Models.Response.Dashboard;
using Cdr.Api.Models.Response.UserStatistics;
using Cdr.Api.Services.Interfaces;
using Common.Enums;

namespace Cdr.Api.Services
{
    public class CdrRecordsService : ICdrRecordsService
    {
        private readonly ICdrRecordsRepository _cdrRecordsRepository;
        private readonly IMapper _mapper;

        public CdrRecordsService(ICdrRecordsRepository cdrRecordsRepository, IMapper mapper)
        {
            _cdrRecordsRepository = cdrRecordsRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CdrRecord>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _cdrRecordsRepository.GetByDateRangeAsync(startDate, endDate);
        }

        public async Task<BarChartResponse<double>> GetAnsweredCallsAsync(ReportPeriod period)
        {
            // Use Turkey timezone for consistent date calculations
            DateTime endDate = TurkeyTimeProvider.Today;
            DateTime startDate = period switch
            {
                ReportPeriod.Weekly => endDate.AddDays(-7),
                ReportPeriod.Monthly => endDate.AddMonths(-1),
                ReportPeriod.Yearly => endDate.AddYears(-1),
                _ => throw new ArgumentOutOfRangeException(nameof(period), period, null)
            };

            return period switch
            {
                ReportPeriod.Weekly => await GetWeeklyAnsweredCallsAsync(startDate, endDate),
                ReportPeriod.Monthly => await GetMonthlyAnsweredCallsAsync(startDate, endDate),
                ReportPeriod.Yearly => await GetYearlyAnsweredCallsAsync(startDate, endDate),
                _ => throw new ArgumentOutOfRangeException(nameof(period), period, null)
            };
        }

        public async Task<DailyCallReport> GetDailyCallReportAsync(DateTime? date = null)
        {
            // Use Turkey timezone for the daily report if no date provided
            return await _cdrRecordsRepository.GetDailyCallReportAsync(date: date ?? TurkeyTimeProvider.Today);
        }

        private async Task<BarChartResponse<double>> GetWeeklyAnsweredCallsAsync(DateTime startDate, DateTime endDate)
        {
            var response = await _cdrRecordsRepository.GetWeeklyAnsweredCallsAsync(startDate, endDate);
            return response.ConvertToBarChartResponse<double, WeeklyAnsweredCallRate>(_mapper);
        }

        private async Task<BarChartResponse<double>> GetMonthlyAnsweredCallsAsync(DateTime startDate, DateTime endDate)
        {
            var response = await _cdrRecordsRepository.GetMonthlyAnsweredCallsAsync(startDate, endDate);
            return response.ConvertToBarChartResponse<double, MonthlyAnsweredCallRate>(_mapper);
        }

        private async Task<BarChartResponse<double>> GetYearlyAnsweredCallsAsync(DateTime startDate, DateTime endDate)
        {
            var response = await _cdrRecordsRepository.GetYearlyAnsweredCallsAsync(startDate, endDate);
            return response.ConvertToBarChartResponse<double, YearlyAnsweredCallRate>(_mapper);
        }

        public async Task<PagedResult<CdrListItem>> GetCallReportListAsync(CdrFilter filter)
        {
            return await _cdrRecordsRepository.GetCallReportListAsync(filter);
        }

        public async Task<NumberStatistics> GetNumberStatisticsByNumberAsync(string number, DateTime startDate, DateTime endDate)
        {
            return await _cdrRecordsRepository.GetNumberStatisticsByNumberAsync(number, startDate, endDate);
        }

        public async Task<PagedResult<UserCallListItem>> GetUserLastCallsAsync(UserStatisticsFilter filter)
        {
            return await _cdrRecordsRepository.GetUserLastCallsAsync(filter);
        }

        public async Task<DepartmentCallStatisticsByCallDirection> GetDepartmentCallStatisticsAsync(DateTime startDate, DateTime endDate)
        {
            return await _cdrRecordsRepository.GetDepartmentCallStatisticsAsync(startDate, endDate);
        }

        public async Task<UserSpecificReport> GetUserCalls(StatisticsFilter filter)
        {
            return await _cdrRecordsRepository.GetUserCalls(filter);
        }

        public async Task<CallStatistics> GetWorkHoursStatistics(StatisticsFilter filter)
        {
            var report = await _cdrRecordsRepository.GetUserCalls(filter);
            return report.WorkHoursStatistics;
        }

        public async Task<CallStatistics> GetNonWorkHoursStatistics(StatisticsFilter filter)
        {
            var report = await _cdrRecordsRepository.GetUserCalls(filter);
            return report.NonWorkHoursStatistics;
        }

        public async Task<List<BreakTime>> GetBreakTimes(StatisticsFilter filter)
        {
            var report = await _cdrRecordsRepository.GetUserCalls(filter);
            return report.BreakTimes;
        }
    }
}