using MicroMediator;

namespace Haiku.Modules.Auth.Commands;

/// <summary>
/// Registers a new user account, returning the created <see cref="User"/> or <c>null</c> if the email or username is already taken.
/// </summary>
/// <param name="Email">The email address for the new account.</param>
/// <param name="Username">The unique display username (3-50 characters, alphanumeric and underscores).</param>
/// <param name="Password">The plain-text password (minimum 8 characters).</param>
public record RegisterUserCommand(string Email, string Username, string Password) : ICommand<User?>;
