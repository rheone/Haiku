using Haiku.Domain.Entities;
using Haiku.Domain.Interfaces;

namespace Haiku.Services.Auth;

/// <summary>
/// Handles user authentication with BCrypt password hashing and timing-safe login verification.
/// </summary>
public class AuthService
{
    private readonly IUserRepository _userRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthService"/> class.
    /// </summary>
    /// <param name="userRepository">Repository for persisting and retrieving user entities.</param>
    public AuthService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    /// <summary>
    /// Registers a new user with the specified credentials.
    /// </summary>
    /// <param name="email">The email address. Must not already be registered.</param>
    /// <param name="username">The desired username. Must not already be taken.</param>
    /// <param name="password">The plaintext password. Hashed with BCrypt using work factor 12.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>The newly created <see cref="User"/> if successful; <c>null</c> if the email or username is already in use.</returns>
    public async Task<User?> RegisterAsync(
        string email,
        string username,
        string password,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (await _userRepository.EmailExistsAsync(email, cancellationToken))
        {
            return null;
        }

        if (await _userRepository.UsernameExistsAsync(username, cancellationToken))
        {
            return null;
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            Username = username,
            DisplayName = username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12),
            CreatedAt = DateTime.UtcNow,
        };

        await _userRepository.SaveAsync(user, cancellationToken);
        return user;
    }

    /// <summary>
    /// Authenticates a user by email and password.
    /// </summary>
    /// <remarks>
    /// <para>
    /// When the email is not found, a dummy BCrypt hash is computed to maintain
    /// constant response time and prevent user enumeration via timing side-channels.
    /// </para>
    /// <para>Disabled and soft-deleted users are denied login.</para>
    /// </remarks>
    /// <param name="email">The registered email address.</param>
    /// <param name="password">The plaintext password to verify.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>The authenticated <see cref="User"/> if credentials are valid and the user is not disabled or deleted; otherwise <c>null</c>.</returns>
    public async Task<User?> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);

        if (user == null)
        {
            // Hash dummy password to prevent timing-based user enumeration.
            BCrypt.Net.BCrypt.HashPassword("dummy", workFactor: 12);
            return null;
        }

        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            return null;
        }

        if (user.IsDisabled || user.IsDeleted)
        {
            return null;
        }

        return user;
    }
}
