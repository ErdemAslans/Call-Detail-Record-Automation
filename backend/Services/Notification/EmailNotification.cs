using System.Net.Mail;
using Cdr.Api.Models;
using Interfaces.Notification;
using Microsoft.Extensions.Options;

public class EmailNotification : INotification<EmailMessage>
{
    private readonly EmailSettings _settings;
    private readonly SemaphoreSlim _sendLock = new(1, 1);

    public EmailNotification(IOptions<EmailSettings> emailSettings)
    {
        _settings = emailSettings.Value;
    }

    public async Task SendAsync(EmailMessage email)
    {
        foreach (var to in email.To)
        {
            await _sendLock.WaitAsync();
            try
            {
                using var smtpClient = new SmtpClient(_settings.Host, _settings.Port)
                {
                    UseDefaultCredentials = false,
                    EnableSsl = false
                };

                if (!string.IsNullOrEmpty(_settings.UserName) && !string.IsNullOrEmpty(_settings.Password))
                {
                    smtpClient.Credentials = new System.Net.NetworkCredential(_settings.UserName, _settings.Password);
                }

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_settings.From),
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
            finally
            {
                _sendLock.Release();
            }
        }
    }
}
