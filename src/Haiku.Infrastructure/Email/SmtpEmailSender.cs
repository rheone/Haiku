using Haiku.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Haiku.Infrastructure.Email;

/// <summary>
/// SMTP-based email sender for production use. Logs a warning that SMTP is not yet configured;
/// actual SMTP delivery requires additional configuration (server, port, credentials).
/// </summary>
public class SmtpEmailSender : IEmailSender
{
    private readonly ILogger<SmtpEmailSender> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="SmtpEmailSender"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    public SmtpEmailSender(ILogger<SmtpEmailSender> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Logs a warning that the email was not sent because SMTP is not configured.
    /// </summary>
    /// <param name="to">The recipient email address.</param>
    /// <param name="subject">The email subject line.</param>
    /// <param name="body">The plain-text email body.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>A completed <see cref="Task"/> representing the no-op operation.</returns>
    public Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        _logger.LogWarning(
            "SMTP email sender is not configured. Email to {To} with subject {Subject} was not sent. Set Email:Sender:Provider to 'Smtp' and configure SMTP settings.",
            to,
            subject
        );
        return Task.CompletedTask;
    }
}
