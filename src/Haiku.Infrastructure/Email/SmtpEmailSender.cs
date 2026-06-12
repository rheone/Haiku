using Haiku.Domain.Interfaces;

namespace Haiku.Infrastructure.Email;

public class SmtpEmailSender : IEmailSender
{
    public Task SendEmailAsync(string to, string subject, string body)
    {
        throw new NotImplementedException("SMTP email sender requires SMTP server configuration.");
    }
}
