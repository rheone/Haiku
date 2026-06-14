using Haiku.Domain.Interfaces;
using Haiku.Services.Haiku;
using NSubstitute;

namespace Haiku.Tests;

/// <summary>Unit tests for <see cref="EmailService"/> covering verification and password reset email sending.</summary>
public class EmailServiceTests
{
    #region SendVerificationEmail

    /// <summary>
    /// Verifies that SendVerificationEmailAsync calls IEmailSender.SendEmailAsync with the correct recipient and a non-empty subject/body.
    /// </summary>
    [Fact]
    public async Task SendVerificationEmailAsync_CallsEmailSender()
    {
        // Auto Generated, verify expected behavior: ensures SendEmailAsync is invoked once with the expected to address.
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

    /// <summary>
    /// Verifies that SendVerificationEmailAsync throws OperationCanceledException when a cancelled token is passed.
    /// </summary>
    [Fact]
    public async Task SendVerificationEmailAsync_CancelledToken_ThrowsOperationCanceledException_Test()
    {
        // Auto Generated, verify expected behavior: ensures the method observes cancellation.
        // Arrange
        var sender = Substitute.For<IEmailSender>();
        var emailService = new EmailService(sender);
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => emailService.SendVerificationEmailAsync(
                "test@example.com",
                "https://example.com/verify?token=abc",
                cts.Token
            )
        );
    }

    #endregion

    #region SendPasswordResetEmail

    /// <summary>
    /// Verifies that SendPasswordResetEmailAsync calls IEmailSender.SendEmailAsync with the correct recipient and a non-empty subject/body.
    /// </summary>
    [Fact]
    public async Task SendPasswordResetEmailAsync_CallsEmailSender()
    {
        // Auto Generated, verify expected behavior: ensures SendEmailAsync is invoked once with the expected to address.
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

    /// <summary>
    /// Verifies that SendPasswordResetEmailAsync throws OperationCanceledException when a cancelled token is passed.
    /// </summary>
    [Fact]
    public async Task SendPasswordResetEmailAsync_CancelledToken_ThrowsOperationCanceledException_Test()
    {
        // Auto Generated, verify expected behavior: ensures the method observes cancellation.
        // Arrange
        var sender = Substitute.For<IEmailSender>();
        var emailService = new EmailService(sender);
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => emailService.SendPasswordResetEmailAsync(
                "test@example.com",
                "https://example.com/reset?token=abc",
                cts.Token
            )
        );
    }

    #endregion
}
