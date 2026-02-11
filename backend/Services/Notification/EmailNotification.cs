using System.Net.Mail;
using Cdr.Api.Models;
using Interfaces.Notification;
using Microsoft.Extensions.Options;

public class EmailNotification : INotification<EmailMessage>
{
    private readonly SmtpClient _smtpClient;
    private readonly string _from;

    public EmailNotification(IOptions<EmailSettings> emailSettings)
    {
        var settings = emailSettings.Value;
        
        _smtpClient = new SmtpClient(settings.Host, settings.Port)
        {
            UseDefaultCredentials = false,
            EnableSsl = false // No SSL for relay
        };
        
        // Only set credentials if username/password are provided
        if (!string.IsNullOrEmpty(settings.UserName) && !string.IsNullOrEmpty(settings.Password))
        {
            _smtpClient.Credentials = new System.Net.NetworkCredential(settings.UserName, settings.Password);
        }

        _from = settings.From;
    }

    public async Task SendAsync(EmailMessage email)
    {
        foreach (var to in email.To)
        {
            using var mailMessage = new MailMessage
            {
                From = new MailAddress(_from),
                Subject = email.Subject,
                Body = email.Body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(to);

            // Add attachments if any
            foreach (var attachmentPath in email.Attachments)
            {
                if (File.Exists(attachmentPath))
                {
                    mailMessage.Attachments.Add(new Attachment(attachmentPath));
                }
            }

            await _smtpClient.SendMailAsync(mailMessage);
        }
    }
}