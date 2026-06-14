using Haiku.Domain.Interfaces;
using Haiku.Services.Slices.Email;
using NSubstitute;

namespace Haiku.Tests.Slices.Email;

/// <summary>Unit tests for <see cref="SendVerificationEmailCommandHandler"/>.</summary>
public class SendVerificationEmailCommandHandlerTests
{
    #region Handle

    /// <summary>Verifies that a verification email is sent with the correct recipient and subject.</summary>
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
        await sender
            .Received(1)
            .SendEmailAsync(
                "test@example.com",
                Arg.Is<string>(s => s.Contains("Verify")),
                Arg.Any<string>(),
                TestContext.Current.CancellationToken
            );
    }

    /// <summary>Verifies that <see cref="OperationCanceledException"/> is thrown when the cancellation token is cancelled.</summary>
    /// <remarks>Auto Generated, verify expected behavior:</remarks>
    [Fact]
    public async Task Handle_ThrowsOperationCanceledException_WhenCancelled_Test()
    {
        // Arrange
        var sender = Substitute.For<IEmailSender>();
        var handler = new SendVerificationEmailCommandHandler(sender);
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            handler.Handle(
                new SendVerificationEmailCommand("test@example.com", "https://example.com/verify?token=abc"),
                cts.Token
            )
        );
    }

    #endregion
}
