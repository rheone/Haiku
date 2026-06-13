using Haiku.Domain.Entities;
using MicroMediator;

namespace Haiku.Services.Slices.Auth;

/// <summary>
/// Authenticates a user by email and password, returning the <see cref="User"/> on success or <c>null</c> on failure.
/// </summary>
/// <param name="Email">The registered email address.</param>
/// <param name="Password">The plain-text password to verify.</param>
public record LoginUserQuery(string Email, string Password) : IQuery<User?>;
