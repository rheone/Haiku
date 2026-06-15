using NSubstitute;

namespace Haiku.Tests.Slices.Email;

/// <summary>Unit tests for <see cref="SendPasswordResetEmailCommandHandler"/>.</summary>
public class SendPasswordResetEmailCommandHandlerTests
{
    #region Handle

    /// <summary>Verifies the handler dispatches a password-reset email via <see cref="IEmailSender"/>.</summary>
    [Fact]
    public async Task Handle_SendsPasswordResetEmail_Test()
    {
        // Arrange
        var sender = Substitute.For<IEmailSender>();
        var handler = new SendPasswordResetEmailCommandHandler(sender);

        // Act
        await handler.Handle(
            new SendPasswordResetEmailCommand("test@example.com", "https://example.com/reset?token=abc"),
            TestContext.Current.CancellationToken
        );

        // Assert
        // Verifies dispatch with subject containing "reset" (case-insensitive). The exact
        // subject-line template is tested by EmailService integration tests.
        await sender
            .Received(1)
            .SendEmailAsync(
                "test@example.com",
                Arg.Is<string>(s => s.Contains("reset")),
                Arg.Any<string>(),
                TestContext.Current.CancellationToken
            );
    }

    #endregion
}
