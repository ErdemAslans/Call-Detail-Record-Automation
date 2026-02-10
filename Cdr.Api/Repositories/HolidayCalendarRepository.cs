using Cdr.Api.Context;
using Cdr.Api.Interfaces;
using Cdr.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cdr.Api.Repositories;

/// <summary>
/// Repository implementation for HolidayCalendar entity.
/// </summary>
public class HolidayCalendarRepository : IHolidayCalendarRepository
{
    private readonly CdrContext _context;

    public HolidayCalendarRepository(CdrContext context)
    {
        _context = context;
    }

    public async Task<HolidayCalendar?> GetByIdAsync(int id)
    {
        return await _context.HolidayCalendars.FindAsync(id);
    }

    public async Task<List<HolidayCalendar>> GetByYearAsync(int year)
    {
        return await _context.HolidayCalendars
            .Where(h => h.IsActive && (h.Year == year || h.IsRecurring))
            .OrderBy(h => h.HolidayDate)
            .ToListAsync();
    }

    public async Task<List<HolidayCalendar>> GetByDateRangeAsync(DateOnly startDate, DateOnly endDate)
    {
        return await _context.HolidayCalendars
            .Where(h => h.IsActive && h.HolidayDate >= startDate && h.HolidayDate <= endDate)
            .OrderBy(h => h.HolidayDate)
            .ToListAsync();
    }

    public async Task<bool> IsHolidayAsync(DateOnly date)
    {
        // Check exact date match
        var exactMatch = await _context.HolidayCalendars
            .AnyAsync(h => h.IsActive && h.HolidayDate == date);
        
        if (exactMatch) return true;

        // Check recurring holidays (same month and day from any year)
        var recurringMatch = await _context.HolidayCalendars
            .AnyAsync(h => h.IsActive && h.IsRecurring 
                && h.HolidayDate.Month == date.Month 
                && h.HolidayDate.Day == date.Day);

        return recurringMatch;
    }

    public async Task<HolidayCalendar?> GetHolidayAsync(DateOnly date)
    {
        // First check exact date match
        var exactMatch = await _context.HolidayCalendars
            .FirstOrDefaultAsync(h => h.IsActive && h.HolidayDate == date);
        
        if (exactMatch != null) return exactMatch;

        // Check recurring holidays
        var recurringMatch = await _context.HolidayCalendars
            .FirstOrDefaultAsync(h => h.IsActive && h.IsRecurring 
                && h.HolidayDate.Month == date.Month 
                && h.HolidayDate.Day == date.Day);

        return recurringMatch;
    }

    public async Task<List<DateOnly>> GetHolidayDatesInRangeAsync(DateOnly startDate, DateOnly endDate)
    {
        var holidays = new HashSet<DateOnly>();

        // Get non-recurring holidays in range
        var directHolidays = await _context.HolidayCalendars
            .Where(h => h.IsActive && !h.IsRecurring && h.HolidayDate >= startDate && h.HolidayDate <= endDate)
            .Select(h => h.HolidayDate)
            .ToListAsync();

        foreach (var date in directHolidays)
        {
            holidays.Add(date);
        }

        // Get recurring holidays and apply them to all years in range
        var recurringHolidays = await _context.HolidayCalendars
            .Where(h => h.IsActive && h.IsRecurring)
            .ToListAsync();

        // For each year in the range, add recurring holiday dates
        for (int year = startDate.Year; year <= endDate.Year; year++)
        {
            foreach (var recurring in recurringHolidays)
            {
                try
                {
                    var holidayDate = new DateOnly(year, recurring.HolidayDate.Month, recurring.HolidayDate.Day);
                    if (holidayDate >= startDate && holidayDate <= endDate)
                    {
                        holidays.Add(holidayDate);
                    }
                }
                catch (ArgumentOutOfRangeException)
                {
                    // Handle Feb 29 on non-leap years - skip
                }
            }
        }

        return holidays.OrderBy(d => d).ToList();
    }

    public async Task<HolidayCalendar> AddAsync(HolidayCalendar holiday)
    {
        _context.HolidayCalendars.Add(holiday);
        await _context.SaveChangesAsync();
        return holiday;
    }

    public async Task<List<HolidayCalendar>> AddRangeAsync(List<HolidayCalendar> holidays)
    {
        _context.HolidayCalendars.AddRange(holidays);
        await _context.SaveChangesAsync();
        return holidays;
    }

    public async Task<HolidayCalendar> UpdateAsync(HolidayCalendar holiday)
    {
        _context.HolidayCalendars.Update(holiday);
        await _context.SaveChangesAsync();
        return holiday;
    }

    public async Task DeleteAsync(int id)
    {
        var holiday = await _context.HolidayCalendars.FindAsync(id);
        if (holiday != null)
        {
            holiday.IsActive = false;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<HolidayCalendar>> GetAllAsync(bool includeInactive = false)
    {
        var query = _context.HolidayCalendars.AsQueryable();
        
        if (!includeInactive)
        {
            query = query.Where(h => h.IsActive);
        }

        return await query.OrderBy(h => h.HolidayDate).ToListAsync();
    }
}
