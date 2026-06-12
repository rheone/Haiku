using Haiku.Domain.Entities;
using Haiku.Domain.Interfaces;
using NHibernate;
using NHibernate.Linq;

namespace Haiku.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ISession _session;

    public UserRepository(ScopedSession scopedSession)
    {
        _session = scopedSession.Session;
    }

    public async Task<User?> GetByIdAsync(Guid id) =>
        await _session.GetAsync<User>(id);

    public async Task<User?> GetByEmailAsync(string email) =>
        await _session.Query<User>().FirstOrDefaultAsync(u => u.Email == email);

    public async Task<User?> GetByUsernameAsync(string username) =>
        await _session.Query<User>().FirstOrDefaultAsync(u => u.Username == username);

    public async Task<bool> EmailExistsAsync(string email) =>
        await _session.Query<User>().AnyAsync(u => u.Email == email);

    public async Task<bool> UsernameExistsAsync(string username) =>
        await _session.Query<User>().AnyAsync(u => u.Username == username);

    public async Task SaveAsync(User user)
    {
        await _session.SaveOrUpdateAsync(user);
        await _session.FlushAsync();
    }

    public async Task UpdateAsync(User user)
    {
        await _session.UpdateAsync(user);
        await _session.FlushAsync();
    }

    public async Task DeleteAsync(User user)
    {
        await _session.DeleteAsync(user);
        await _session.FlushAsync();
    }
}
