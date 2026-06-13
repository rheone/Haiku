using Haiku.Domain.Interfaces;
using MicroMediator;

namespace Haiku.Services.Slices.Email;

/// <summary>
/// Handles sending password reset emails via the configured <see cref="IEmailSender"/>.
/// </summary>
public class SendPasswordResetEmailCommandHandler : ICommandHandler<SendPasswordResetEmailCommand>
{
    private readonly IEmailSender _emailSender;

    /// <summary>
    /// Initializes a new instance of the <see cref="SendPasswordResetEmailCommandHandler"/> class.
    /// </summary>
    /// <param name="emailSender">The email sender used to dispatch the message.</param>
    public SendPasswordResetEmailCommandHandler(IEmailSender emailSender)
    {
        _emailSender = emailSender;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// <para>
    /// Composes a "Haiku password reset" email containing the reset link and dispatches it
    /// via <see cref="IEmailSender"/>.
    /// </para>
    /// </remarks>
    public async Task Handle(SendPasswordResetEmailCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var subject = "Haiku password reset";
        var body = $"Click here to reset your password: {request.ResetLink}";
        await _emailSender.SendEmailAsync(request.To, subject, body, cancellationToken);
    }
}
