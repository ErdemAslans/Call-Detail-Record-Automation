namespace Cdr.Api.Models;

public class EmailSettings
{
    public required string Host { get; set; }
    
    public int Port { get; set; }

    public int SslPort { get; set; }
    
    public string? UserName { get; set; }
    
    public string? Password { get; set; }
    
    public required string From { get; set; }
    
    public required string Subject { get; set; }
}
