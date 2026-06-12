using Haiku.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Haiku.Infrastructure.Email;

public class ConsoleEmailSender : IEmailSender
{
    private readonly ILogger<ConsoleEmailSender> _logger;

    public ConsoleEmailSender(ILogger<ConsoleEmailSender> logger)
    {
        _logger = logger;
    }

    public Task SendEmailAsync(string to, string subject, string body)
    {
        _logger.LogInformation("[EMAIL] To: {To}", to);
        _logger.LogInformation("[EMAIL] Subject: {Subject}", subject);
        _logger.LogInformation("[EMAIL] Body: {Body}", body);
        return Task.CompletedTask;
    }
}
