using System.ComponentModel.DataAnnotations;

namespace Cdr.Api.Models;

public class DateRangeFilter
{
    [Required(ErrorMessage = "Başlangıç tarihi zorunludur")]
    public DateTime StartDate { get; set; }

    [Required(ErrorMessage = "Bitiş tarihi zorunludur")]
    public DateTime EndDate { get; set; }
} 