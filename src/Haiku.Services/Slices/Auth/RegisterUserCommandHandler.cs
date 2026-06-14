using Haiku.Domain.Entities;
using Haiku.Domain.Interfaces;
using MicroMediator;

namespace Haiku.Services.Slices.Auth;

/// <summary>
/// Handles user registration, enforcing unique email and username constraints.
/// Returns <c>null</c> on conflict rather than throwing, matching the CQRS query pattern.
/// </summary>
public class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand, User?>
{
    private readonly IUserRepository _userRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="RegisterUserCommandHandler"/> class.
    /// </summary>
    /// <param name="userRepository">The user repository for data access.</param>
    public RegisterUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    /// <inheritdoc/>
    public async Task<User?> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (await _userRepository.EmailExistsAsync(request.Email, cancellationToken))
        {
            return null;
        }

        if (await _userRepository.UsernameExistsAsync(request.Username, cancellationToken))
        {
            return null;
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            Username = request.Username,
            // Default display name to username; user can customize later in profile settings.
            DisplayName = request.Username,
            // Work factor 12 targets ~300ms per hash, balancing brute-force resistance with UX.
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password, workFactor: 12),
            CreatedAt = DateTime.UtcNow,
        };

        await _userRepository.SaveAsync(user, cancellationToken);
        return user;
    }
}
