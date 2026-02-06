namespace Cdr.Api.Models;

public class SmsMessage
{
    public required string To { get; set; }

    public required string Body { get; set; }
}