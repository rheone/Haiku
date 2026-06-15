using NSubstitute;

namespace Haiku.Tests;

/// <summary>Unit tests for <see cref="EmailService"/> covering verification and password reset email sending.</summary>
public class EmailServiceTests
{
    /// <summary>Verifies that <see cref="EmailService.SendVerificationEmailAsync"/> dispatches the email through <see cref="IEmailSender.SendEmailAsync"/>.</summary>
    [Fact]
    public async Task SendVerificationEmailAsync_CallsEmailSender_Test()
    {
        // Arrange
        var sender = Substitute.For<IEmailSender>();
        var emailService = new EmailService(sender);

        // Act
        await emailService.SendVerificationEmailAsync(
            "test@example.com",
            "https://example.com/verify?token=abc",
            TestContext.Current.CancellationToken
        );

        // Assert
        await sender
            .Received(1)
            .SendEmailAsync("test@example.com", Arg.Any<string>(), Arg.Any<string>(), TestContext.Current.CancellationToken);
    }

    /// <summary>Verifies that <see cref="EmailService.SendPasswordResetEmailAsync"/> dispatches the email through <see cref="IEmailSender.SendEmailAsync"/>.</summary>
    [Fact]
    public async Task SendPasswordResetEmailAsync_CallsEmailSender_Test()
    {
        // Arrange
        var sender = Substitute.For<IEmailSender>();
        var emailService = new EmailService(sender);

        // Act
        await emailService.SendPasswordResetEmailAsync(
            "test@example.com",
            "https://example.com/reset?token=abc",
            TestContext.Current.CancellationToken
        );

        // Assert
        await sender
            .Received(1)
            .SendEmailAsync("test@example.com", Arg.Any<string>(), Arg.Any<string>(), TestContext.Current.CancellationToken);
    }
}
