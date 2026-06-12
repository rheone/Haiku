using Haiku.Domain.Entities;
using Haiku.Domain.Interfaces;
using NHibernate;
using NHibernate.Linq;

namespace Haiku.Infrastructure.Repositories;

public class VoteRepository : IVoteRepository
{
    private readonly ISession _session;

    public VoteRepository(ScopedSession scopedSession)
    {
        _session = scopedSession.Session;
    }

    public async Task<Vote?> GetByUserAndHaikuAsync(Guid userId, Guid haikuId) =>
        await _session.Query<Vote>()
            .FirstOrDefaultAsync(v => v.User.Id == userId && v.Poem.Id == haikuId);

    public async Task SaveAsync(Vote vote)
    {
        await _session.SaveOrUpdateAsync(vote);
        await _session.FlushAsync();
    }

    public async Task DeleteAsync(Vote vote)
    {
        await _session.DeleteAsync(vote);
        await _session.FlushAsync();
    }

    public async Task<int> GetNetScoreAsync(Guid haikuId)
    {
        var result = await _session.Query<Vote>()
            .Where(v => v.Poem.Id == haikuId)
            .Select(v => (int)v.Value)
            .ToListAsync();

        return result.Sum();
    }
}
