namespace Haiku.Domain.Tests.Entities;

/// <summary>
/// Unit tests for <see cref="Haiku.Domain.Entities.UserVerificationToken"/> entity instantiation and property assignment.
/// Email verification tokens are time-limited, single-use cryptographic tokens issued during
/// registration. Tokens are invalidated upon successful verification or expiration, and
/// the token string is hashed before storage.
/// </summary>
public class UserVerificationTokenTests { }
