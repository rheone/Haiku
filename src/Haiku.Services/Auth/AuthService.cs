using Haiku.Domain.Entities;
using Haiku.Domain.Interfaces;

namespace Haiku.Services.Auth;

public class AuthService
{
    private readonly IUserRepository _userRepository;

    public AuthService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User?> RegisterAsync(string email, string username, string password)
    {
        if (await _userRepository.EmailExistsAsync(email))
            return null;

        if (await _userRepository.UsernameExistsAsync(username))
            return null;

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            Username = username,
            DisplayName = username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12),
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.SaveAsync(user);
        return user;
    }

    public async Task<User?> LoginAsync(string email, string password)
    {
        var user = await _userRepository.GetByEmailAsync(email);

        if (user == null)
        {
            BCrypt.Net.BCrypt.HashPassword("dummy", workFactor: 12);
            return null;
        }

        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return null;

        if (user.IsDisabled || user.IsDeleted)
            return null;

        return user;
    }
}
