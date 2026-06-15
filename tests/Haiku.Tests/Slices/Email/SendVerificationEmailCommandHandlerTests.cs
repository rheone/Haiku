using NSubstitute;

namespace Haiku.Tests.Slices.Email;

/// <summary>Unit tests for <see cref="SendVerificationEmailCommandHandler"/>.</summary>
public class SendVerificationEmailCommandHandlerTests
{
    #region Handle

    /// <summary>Verifies the handler dispatches a verification email via <see cref="IEmailSender"/>.</summary>
    [Fact]
    public async Task Handle_SendsVerificationEmail_Test()
    {
        // Arrange
        var sender = Substitute.For<IEmailSender>();
        var handler = new SendVerificationEmailCommandHandler(sender);

        // Act
        await handler.Handle(
            new SendVerificationEmailCommand("test@example.com", "https://example.com/verify?token=abc"),
            TestContext.Current.CancellationToken
        );

        // Assert
        // Verifies dispatch with subject containing "Verify" (capitalized per the template).
        // The exact subject-line template is tested by EmailService integration tests.
        await sender
            .Received(1)
            .SendEmailAsync(
                "test@example.com",
                Arg.Is<string>(s => s.Contains("Verify")),
                Arg.Any<string>(),
                TestContext.Current.CancellationToken
            );
    }

    #endregion
}
