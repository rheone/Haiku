namespace Haiku.Infrastructure.Email;

/// <summary>
/// Represents an email message with recipient, subject, and body content.
/// </summary>
/// <param name="To">The recipient email address.</param>
/// <param name="Subject">The email subject line.</param>
/// <param name="Body">The plain-text email body content.</param>
public record EmailMessage(string To, string Subject, string Body);
