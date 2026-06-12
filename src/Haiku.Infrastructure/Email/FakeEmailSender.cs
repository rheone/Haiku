using Haiku.Domain.Interfaces;

namespace Haiku.Infrastructure.Email;

public class FakeEmailSender : IEmailSender
{
    public List<EmailMessage> SentEmails { get; } = new();

    public Task SendEmailAsync(string to, string subject, string body)
    {
        SentEmails.Add(new EmailMessage(to, subject, body));
        return Task.CompletedTask;
    }

    public void Clear() => SentEmails.Clear();
}

public record EmailMessage(string To, string Subject, string Body);
