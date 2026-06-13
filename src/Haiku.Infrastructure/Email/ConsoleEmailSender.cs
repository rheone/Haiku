using Haiku.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Haiku.Infrastructure.Email;

/// <summary>
/// Sends email by logging it to the console via <see cref="ILogger{TCategoryName}"/>.
/// Used in development environments where no SMTP server is available.
/// </summary>
public class ConsoleEmailSender : IEmailSender
{
    private readonly ILogger<ConsoleEmailSender> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConsoleEmailSender"/> class.
    /// </summary>
    /// <param name="logger">The logger instance for writing email content.</param>
    public ConsoleEmailSender(ILogger<ConsoleEmailSender> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Logs the email recipient, subject, and body to the console.
    /// </summary>
    /// <param name="to">The recipient email address.</param>
    /// <param name="subject">The email subject line.</param>
    /// <param name="body">The plain-text email body.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>A completed <see cref="Task"/> representing the synchronous log operation.</returns>
    public Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[EMAIL] To: {To}", to);
        _logger.LogInformation("[EMAIL] Subject: {Subject}; {Body}", subject, body);
        return Task.CompletedTask;
    }
}
