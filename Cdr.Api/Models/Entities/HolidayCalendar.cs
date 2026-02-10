using System.ComponentModel.DataAnnotations;

namespace Cdr.Api.Models.Entities;

/// <summary>
/// Stores Turkey public holidays for after-hours call classification.
/// Used to determine if a call occurred during a holiday (treated as after-hours).
/// Per FR-008 and Assumption §10.
/// </summary>
public class HolidayCalendar
{
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// The date of the holiday (date only, no time component)
    /// </summary>
    [Required]
    public DateOnly HolidayDate { get; set; }

    /// <summary>
    /// Name of the holiday in Turkish
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string HolidayName { get; set; } = string.Empty;

    /// <summary>
    /// Name of the holiday in English (optional)
    /// </summary>
    [MaxLength(200)]
    public string? HolidayNameEn { get; set; }

    /// <summary>
    /// Type of holiday: Public, Religious, National, Custom
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string HolidayType { get; set; } = "Public";

    /// <summary>
    /// Whether this is an annual recurring holiday
    /// </summary>
    public bool IsRecurring { get; set; } = false;

    /// <summary>
    /// For recurring holidays, the month (1-12)
    /// </summary>
    public int? RecurringMonth { get; set; }

    /// <summary>
    /// For recurring holidays, the day of month (1-31)
    /// </summary>
    public int? RecurringDay { get; set; }

    /// <summary>
    /// Year this holiday entry applies to (for non-recurring)
    /// </summary>
    public int Year { get; set; }

    /// <summary>
    /// Whether this holiday is active/enabled
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Optional notes about the holiday
    /// </summary>
    [MaxLength(500)]
    public string? Notes { get; set; }

    /// <summary>
    /// Record creation timestamp (UTC)
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Record last update timestamp (UTC)
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
