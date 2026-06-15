namespace Haiku.Modules.Shared.Infrastructure.Email;

// Abstraction over email delivery with implementations for SMTP (production)
// and a no-op fake for development (see FakeEmailSender in tests).

/// <summary>
/// Provides an abstraction for sending email notifications.
/// </summary>
/// <remarks>
/// <para>Email is used for account-related notifications (registration confirmation,
/// password reset). The interface has a single sending method to keep implementations
/// simple. Production uses SMTP via <c>SmtpEmailSender</c>; development and tests use
/// <c>FakeEmailSender</c> which logs the message without delivering it.</para>
/// </remarks>
public interface IEmailSender
{
    /// <summary>
    /// Sends an email message to the specified recipient.
    /// </summary>
    /// <param name="to">The recipient email address.</param>
    /// <param name="subject">The email subject line.</param>
    /// <param name="body">The plain-text email body.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default);
}
