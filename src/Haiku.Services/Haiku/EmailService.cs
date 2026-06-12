using Haiku.Domain.Interfaces;

namespace Haiku.Services.Haiku;

public class EmailService
{
    private readonly IEmailSender _emailSender;

    public EmailService(IEmailSender emailSender)
    {
        _emailSender = emailSender;
    }

    public async Task SendVerificationEmailAsync(string to, string verificationLink)
    {
        var subject = "Verify your Haiku account";
        var body = $"Welcome to Haiku! Please verify your email by clicking: {verificationLink}";
        await _emailSender.SendEmailAsync(to, subject, body);
    }

    public async Task SendPasswordResetEmailAsync(string to, string resetLink)
    {
        var subject = "Haiku password reset";
        var body = $"Click here to reset your password: {resetLink}";
        await _emailSender.SendEmailAsync(to, subject, body);
    }
}
