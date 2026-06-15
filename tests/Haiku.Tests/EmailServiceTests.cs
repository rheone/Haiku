using NSubstitute;

namespace Haiku.Tests;

/// <summary>Unit tests for <see cref="EmailService"/> covering verification and password reset email sending.</summary>
public class EmailServiceTests
{
    [Fact]
    public async Task SendVerificationEmailAsync_CallsEmailSender()
    {
        var sender = Substitute.For<IEmailSender>();
        var emailService = new EmailService(sender);

        await emailService.SendVerificationEmailAsync(
            "test@example.com",
            "https://example.com/verify?token=abc",
            TestContext.Current.CancellationToken
        );

        // Verifies the sender was called; the subject and body are constructed inside EmailService
        // and validated by EmailService integration tests, so we assert only that dispatch occurred.
        await sender
            .Received(1)
            .SendEmailAsync("test@example.com", Arg.Any<string>(), Arg.Any<string>(), TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task SendPasswordResetEmailAsync_CallsEmailSender()
    {
        var sender = Substitute.For<IEmailSender>();
        var emailService = new EmailService(sender);

        await emailService.SendPasswordResetEmailAsync(
            "test@example.com",
            "https://example.com/reset?token=abc",
            TestContext.Current.CancellationToken
        );

        // Verifies the sender was called; the subject and body are constructed inside EmailService
        // and validated by EmailService integration tests, so we assert only that dispatch occurred.
        await sender
            .Received(1)
            .SendEmailAsync("test@example.com", Arg.Any<string>(), Arg.Any<string>(), TestContext.Current.CancellationToken);
    }
}
