using System.Net;
using System.Net.Mail;
using Cdr.Api.Models;
using Interfaces.Notification;
using Microsoft.Extensions.Options;

public class EmailNotification : INotification<EmailMessage>
{
    private readonly EmailSettings _settings;
    private readonly string _from;

    public EmailNotification(IOptions<EmailSettings> emailSettings)
    {
        _settings = emailSettings.Value;
        _from = _settings.From;
    }

    public async Task SendAsync(EmailMessage email)
    {
        foreach (var to in email.To)
        {
            // Create a fresh SmtpClient per send to avoid stale/corrupt connections
            using var smtpClient = new SmtpClient(_settings.Host, _settings.Port)
            {
                UseDefaultCredentials = false,
                EnableSsl = false,
                Timeout = 30000 // 30 second timeout
            };

            if (!string.IsNullOrEmpty(_settings.UserName) && !string.IsNullOrEmpty(_settings.Password))
            {
                smtpClient.Credentials = new NetworkCredential(_settings.UserName, _settings.Password);
            }

            using var mailMessage = new MailMessage
            {
                From = new MailAddress(_from),
                Subject = email.Subject,
                Body = email.Body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(to);

            foreach (var attachmentPath in email.Attachments)
            {
                if (File.Exists(attachmentPath))
                {
                    mailMessage.Attachments.Add(new Attachment(attachmentPath));
                }
            }

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
