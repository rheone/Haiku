using MicroMediator;

namespace Haiku.Services.Slices.Email;

/// <summary>
/// Command to send an email verification link to a user during registration.
/// </summary>
/// <param name="To">The email address of the recipient.</param>
/// <param name="VerificationLink">The verification URL to include in the email body.</param>
public record SendVerificationEmailCommand(string To, string VerificationLink) : ICommand;
