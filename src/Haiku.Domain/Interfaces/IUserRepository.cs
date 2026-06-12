using Haiku.Domain.Entities;

namespace Haiku.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByUsernameAsync(string username);
    Task<bool> EmailExistsAsync(string email);
    Task<bool> UsernameExistsAsync(string username);
    Task SaveAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(User user);
}
