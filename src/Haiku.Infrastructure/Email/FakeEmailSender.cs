using Haiku.Domain.Interfaces;

namespace Haiku.Infrastructure.Email;

/// <summary>
/// Test double for <see cref="IEmailSender"/> that captures sent emails in memory
/// for verification during unit tests.
/// </summary>
public class FakeEmailSender : IEmailSender
{
    /// <summary>
    /// Gets the collection of emails sent during the test session.
    /// </summary>
    /// <value>A <see cref="List{T}"/> of <see cref="EmailMessage"/> instances captured in memory.</value>
    public List<EmailMessage> SentEmails { get; } = new();

    /// <summary>
    /// Captures the email parameters into the <see cref="SentEmails"/> collection without sending.
    /// </summary>
    /// <param name="to">The recipient email address.</param>
    /// <param name="subject">The email subject line.</param>
    /// <param name="body">The plain-text email body.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>A completed <see cref="Task"/> representing the synchronous capture operation.</returns>
    public Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        SentEmails.Add(new EmailMessage(to, subject, body));
        return Task.CompletedTask;
    }

    /// <summary>
    /// Clears all recorded sent emails from the <see cref="SentEmails"/> collection.
    /// </summary>
    public void Clear() => SentEmails.Clear();
}
