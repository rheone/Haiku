using Haiku.Domain.Interfaces;
using Haiku.Services.Slices.Email;
using NSubstitute;

namespace Haiku.Tests.Slices.Email;

/// <summary>Unit tests for <see cref="SendVerificationEmailCommandHandler"/>.</summary>
public class SendVerificationEmailCommandHandlerTests
{
    [Fact]
    public async Task Handle_SendsVerificationEmail()
    {
        var sender = Substitute.For<IEmailSender>();
        var handler = new SendVerificationEmailCommandHandler(sender);

        await handler.Handle(
            new SendVerificationEmailCommand("test@example.com", "https://example.com/verify?token=abc"),
            TestContext.Current.CancellationToken
        );

        await sender
            .Received(1)
            .SendEmailAsync(
                "test@example.com",
                Arg.Is<string>(s => s.Contains("Verify")),
                Arg.Any<string>(),
                TestContext.Current.CancellationToken
            );
    }
}
