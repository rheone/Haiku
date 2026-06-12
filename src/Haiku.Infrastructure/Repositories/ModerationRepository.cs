using Haiku.Domain.Entities;
using Haiku.Domain.Interfaces;
using NHibernate;
using NHibernate.Linq;

namespace Haiku.Infrastructure.Repositories;

public class ModerationRepository : IModerationRepository
{
    private readonly ISession _session;

    public ModerationRepository(ScopedSession scopedSession)
    {
        _session = scopedSession.Session;
    }

    public async Task SaveActionAsync(ModerationAction action)
    {
        await _session.SaveOrUpdateAsync(action);
        await _session.FlushAsync();
    }

    public async Task<List<UserPrivilege>> GetUserPrivilegesAsync(Guid userId) =>
        await _session.Query<UserPrivilege>()
            .Where(p => p.User.Id == userId)
            .ToListAsync();

    public async Task<bool> HasPrivilegeAsync(Guid userId, string privilege) =>
        await _session.Query<UserPrivilege>()
            .AnyAsync(p => p.User.Id == userId && p.Privilege == privilege);

    public async Task GrantPrivilegeAsync(UserPrivilege privilege)
    {
        await _session.SaveOrUpdateAsync(privilege);
        await _session.FlushAsync();
    }

    public async Task RevokePrivilegeAsync(UserPrivilege privilege)
    {
        await _session.DeleteAsync(privilege);
        await _session.FlushAsync();
    }
}
