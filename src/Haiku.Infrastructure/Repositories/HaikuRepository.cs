using Haiku.Domain.Entities;
using Haiku.Domain.Interfaces;
using NHibernate;

namespace Haiku.Infrastructure.Repositories;

public class HaikuRepository : IHaikuRepository
{
    private readonly ISession _session;

    public HaikuRepository(ScopedSession scopedSession)
    {
        _session = scopedSession.Session;
    }

    public async Task<Poem?> GetByIdAsync(Guid id) =>
        await _session.GetAsync<Poem>(id);

    public async Task SaveAsync(Poem poem)
    {
        await _session.SaveOrUpdateAsync(poem);
        await _session.FlushAsync();
    }

    public async Task DeleteAsync(Poem poem)
    {
        await _session.DeleteAsync(poem);
        await _session.FlushAsync();
    }
}
