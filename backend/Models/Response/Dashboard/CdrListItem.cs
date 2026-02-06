using Common.Enums;

namespace Cdr.Api.Models.Response.Dashboard;

public class CdrListItem
{
    public required string Id { get; set; }

    public required string OriginalCalledPartyNumber { get; set; }

    public required string CallingPartyNumber { get; set; }

    public string? FinalCalledPartyNumber { get; set; }

    public DateTime DateTimeOrigination { get; set; }

    public DateTime? DateTimeConnect { get; set; }

    public int? CallAwaitDuration => DateTimeConnect.HasValue ? (DateTimeConnect.Value - DateTimeOrigination).Seconds : null;

    public int? Duration { get; set; }

    public bool HasRedirected { get; set; }

    // public CallEndedReason CallEndedReason { get; set; }

    public CallType CallType { get; set; }

    public RedirectReason? RedirectReason { get; set; }

    public CallDirection? CallDirection { get; set; }

    public string? UserName { get; set; }

    public string? DepartmentName { get; set; }

    public string? FinalCalledPartyUserName { get; set; }

    public string? FinalCalledPartyDepartmentName { get; set; }

    public string? OriginalCalledPartyUserName { get; set; }

    public string? OriginalCalledPartyDepartmentName { get; set; }
}