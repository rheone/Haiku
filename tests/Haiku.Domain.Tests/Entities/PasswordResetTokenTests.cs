namespace Haiku.Domain.Tests.Entities;

/// <summary>
/// Unit tests for <see cref="Haiku.Domain.Entities.PasswordResetToken"/> entity instantiation and property assignment.
/// Password reset tokens are time-limited, single-use cryptographic tokens that authorize
/// a password change. Tokens are invalidated upon use or expiration, and the token string
/// is hashed before storage.
/// </summary>
public class PasswordResetTokenTests { }
