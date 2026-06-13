using Haiku.Domain.Interfaces;
using MicroMediator;

namespace Haiku.Services.Slices.Email;

/// <summary>
/// Handles sending account verification emails via the configured <see cref="IEmailSender"/>.
/// </summary>
public class SendVerificationEmailCommandHandler : ICommandHandler<SendVerificationEmailCommand>
{
    private readonly IEmailSender _emailSender;

    /// <summary>
    /// Initializes a new instance of the <see cref="SendVerificationEmailCommandHandler"/> class.
    /// </summary>
    /// <param name="emailSender">The email sender used to dispatch the message.</param>
    public SendVerificationEmailCommandHandler(IEmailSender emailSender)
    {
        _emailSender = emailSender;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// <para>
    /// Composes a welcome email containing the verification link and dispatches it
    /// via <see cref="IEmailSender"/>.
    /// </para>
    /// </remarks>
    public async Task Handle(SendVerificationEmailCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var subject = "Verify your Haiku account";
        var body = $"Welcome to Haiku! Please verify your email by clicking: {request.VerificationLink}";
        await _emailSender.SendEmailAsync(request.To, subject, body, cancellationToken);
    }
}
