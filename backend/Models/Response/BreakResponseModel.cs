namespace Cdr.Api.Models.Response;

    public class BreakResponseModel
    {
        public required string Id { get; set; }
        public required string UserId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string? Reason { get; set; }
    }

