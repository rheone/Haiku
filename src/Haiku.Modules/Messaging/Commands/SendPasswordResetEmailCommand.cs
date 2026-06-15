using MicroMediator;

namespace Haiku.Modules.Messaging.Commands;

/// <summary>
/// Command to send a password reset email to a user.
/// </summary>
/// <param name="To">The email address of the recipient.</param>
/// <param name="ResetLink">The password reset URL to include in the email body.</param>
public record SendPasswordResetEmailCommand(string To, string ResetLink) : ICommand;
