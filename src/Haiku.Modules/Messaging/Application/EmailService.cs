namespace Haiku.Modules.Messaging.Application;

/// <summary>
/// Sends transactional emails for account verification and password reset flows.
/// </summary>
public class EmailService
{
    private readonly IEmailSender _emailSender;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmailService"/> class.
    /// </summary>
    /// <param name="emailSender">The underlying email sender implementation.</param>
    public EmailService(IEmailSender emailSender)
    {
        _emailSender = emailSender;
    }

    /// <summary>
    /// Sends an email verification link to the specified address.
    /// </summary>
    /// <param name="to">The recipient email address.</param>
    /// <param name="verificationLink">The full URL for email verification.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is cancelled.</exception>
    public async Task SendVerificationEmailAsync(
        string to,
        string verificationLink,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();
        var subject = "Verify your Haiku account";
        var body = $"Welcome to Haiku! Please verify your email by clicking: {verificationLink}";
        await _emailSender.SendEmailAsync(to, subject, body, cancellationToken);
    }

    /// <summary>
    /// Sends a password reset link to the specified address.
    /// </summary>
    /// <param name="to">The recipient email address.</param>
    /// <param name="resetLink">The full URL for password reset.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is cancelled.</exception>
    public async Task SendPasswordResetEmailAsync(string to, string resetLink, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var subject = "Haiku password reset";
        var body = $"Click here to reset your password: {resetLink}";
        await _emailSender.SendEmailAsync(to, subject, body, cancellationToken);
    }
}
