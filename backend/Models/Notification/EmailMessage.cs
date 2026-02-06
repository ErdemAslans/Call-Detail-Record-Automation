namespace Cdr.Api.Models;

public class EmailMessage
{

    public required string Subject { get; set; }

    public required string Body { get; set; }
    
    public List<string> To { get; set; } = new List<string>();

    public List<string> Cc { get; set; } = new List<string>();
    
    public List<string> Bcc { get; set; } = new List<string>();
    
    public List<string> Attachments { get; set; } = new List<string>();
}