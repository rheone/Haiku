namespace Haiku.Domain.Interfaces;

/// <summary>
/// Provides an abstraction for sending email notifications.
/// </summary>
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
