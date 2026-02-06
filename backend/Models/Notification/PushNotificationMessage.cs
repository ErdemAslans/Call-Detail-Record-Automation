namespace Cdr.Api.Models;

public class PushNotificationMessage
{
    public required string To { get; set; }
    
    public required string Body { get; set; }
}