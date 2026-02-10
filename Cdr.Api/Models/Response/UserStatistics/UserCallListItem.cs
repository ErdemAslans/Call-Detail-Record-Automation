using Common.Enums;

namespace Cdr.Api.Models.Response.UserStatistics;

public class UserCallListItem
{
    public required string Id { get; set; }

    public required string CallingPartyNumber { get; set; }

    public required string OriginalCalledPartyNumber { get; set; }

    public required string FinalCalledPartyNumber { get; set; } // Added line

    public CallType CallType { get; set; }

    public int? Duration { get; set; }

    public DateTime DateTimeOrigination { get; set; }

    public DateTime? DateTimeDisconnect { get; set; }

    public string? CallingPartyUserName { get; set; }

    public string? CallingPartyDepartmentName { get; set; }

    public string? OriginalCalledPartyUserName { get; set; }

    public string? OriginalCalledPartyDepartmentName { get; set; }

    public string? FinalCalledPartyUserName { get; set; } // Added line

    public string? FinalCalledPartyDepartmentName { get; set; } // Added line

    public CallDirection? CallDirection { get; set; }
}