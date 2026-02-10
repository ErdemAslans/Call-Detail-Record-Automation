namespace Cdr.Api.Models.Request
{
    public class BreakRequest
    {
        public string? Reason { get; set; }
        public required DateTime PlannedEndTime { get; set; }
    }
}
