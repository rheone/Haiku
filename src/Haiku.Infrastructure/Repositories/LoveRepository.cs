using Haiku.Domain.Entities;
using Haiku.Domain.Interfaces;
using NHibernate;
using NHibernate.Linq;

namespace Haiku.Infrastructure.Repositories;

public class LoveRepository : ILoveRepository
{
    private readonly ISession _session;

    public LoveRepository(ScopedSession scopedSession)
    {
        _session = scopedSession.Session;
    }

    public async Task<Love?> GetByUserAndHaikuAsync(Guid userId, Guid haikuId) =>
        await _session.Query<Love>()
            .FirstOrDefaultAsync(l => l.User.Id == userId && l.Poem.Id == haikuId);

    public async Task SaveAsync(Love love)
    {
        await _session.SaveOrUpdateAsync(love);
        await _session.FlushAsync();
    }

    public async Task DeleteAsync(Love love)
    {
        await _session.DeleteAsync(love);
        await _session.FlushAsync();
    }

    public async Task<int> GetLoveCountAsync(Guid haikuId) =>
        await _session.Query<Love>().CountAsync(l => l.Poem.Id == haikuId);
}
