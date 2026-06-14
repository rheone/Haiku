using Haiku.Domain.Interfaces;
using Haiku.Services.Slices.Email;
using NSubstitute;

namespace Haiku.Tests.Slices.Email;

/// <summary>Unit tests for <see cref="SendPasswordResetEmailCommandHandler"/>.</summary>
public class SendPasswordResetEmailCommandHandlerTests
{
    #region Handle

    /// <summary>Verifies that a password reset email is sent with the correct recipient and subject.</summary>
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
        await sender
            .Received(1)
            .SendEmailAsync(
                "test@example.com",
                Arg.Is<string>(s => s.Contains("reset")),
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
        var handler = new SendPasswordResetEmailCommandHandler(sender);
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            handler.Handle(
                new SendPasswordResetEmailCommand("test@example.com", "https://example.com/reset?token=abc"),
                cts.Token
            )
        );
    }

    #endregion
}
