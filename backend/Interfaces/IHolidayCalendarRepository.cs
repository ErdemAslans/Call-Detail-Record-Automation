using Cdr.Api.Models.Entities;

namespace Cdr.Api.Interfaces;

/// <summary>
/// Repository interface for HolidayCalendar entity operations.
/// </summary>
public interface IHolidayCalendarRepository
{
    /// <summary>
    /// Get a holiday by ID.
    /// </summary>
    Task<HolidayCalendar?> GetByIdAsync(int id);

    /// <summary>
    /// Get all active holidays for a specific year.
    /// </summary>
    Task<List<HolidayCalendar>> GetByYearAsync(int year);

    /// <summary>
    /// Get all active holidays within a date range.
    /// </summary>
    Task<List<HolidayCalendar>> GetByDateRangeAsync(DateOnly startDate, DateOnly endDate);

    /// <summary>
    /// Check if a specific date is a holiday.
    /// </summary>
    Task<bool> IsHolidayAsync(DateOnly date);

    /// <summary>
    /// Get holiday details for a specific date (if it's a holiday).
    /// </summary>
    Task<HolidayCalendar?> GetHolidayAsync(DateOnly date);

    /// <summary>
    /// Get all holidays (including recurring) that apply to a date range.
    /// Handles recurring holidays by checking month/day patterns.
    /// </summary>
    Task<List<DateOnly>> GetHolidayDatesInRangeAsync(DateOnly startDate, DateOnly endDate);

    /// <summary>
    /// Add a new holiday.
    /// </summary>
    Task<HolidayCalendar> AddAsync(HolidayCalendar holiday);

    /// <summary>
    /// Add multiple holidays (bulk seed).
    /// </summary>
    Task<List<HolidayCalendar>> AddRangeAsync(List<HolidayCalendar> holidays);

    /// <summary>
    /// Update an existing holiday.
    /// </summary>
    Task<HolidayCalendar> UpdateAsync(HolidayCalendar holiday);

    /// <summary>
    /// Soft delete a holiday (set IsActive = false).
    /// </summary>
    Task DeleteAsync(int id);

    /// <summary>
    /// Get all holidays (for admin management).
    /// </summary>
    Task<List<HolidayCalendar>> GetAllAsync(bool includeInactive = false);
}
