using Haiku.Domain.Entities;
using Haiku.Domain.Interfaces;
using MicroMediator;

namespace Haiku.Services.Slices.Auth;

/// <summary>
/// Handles user authentication by verifying credentials and account status.
/// Uses a timing-safe comparison pattern to prevent user enumeration via response timing.
/// </summary>
public class LoginUserQueryHandler : IQueryHandler<LoginUserQuery, User?>
{
    private readonly IUserRepository _userRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoginUserQueryHandler"/> class.
    /// </summary>
    /// <param name="userRepository">The user repository for data access.</param>
    public LoginUserQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    /// <inheritdoc/>
    public async Task<User?> Handle(LoginUserQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user == null)
        {
            // Timing-safe dummy hash: prevents attackers from distinguishing
            // "email not found" from "wrong password" by response time.
            BCrypt.Net.BCrypt.HashPassword("dummy", workFactor: 12);
            return null;
        }

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
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
