using Haiku.Domain.Entities;
using Haiku.Domain.Interfaces;
using NHibernate;
using NHibernate.Linq;

namespace Haiku.Infrastructure.Repositories;

public class BookmarkRepository : IBookmarkRepository
{
    private readonly ISession _session;

    public BookmarkRepository(ScopedSession scopedSession)
    {
        _session = scopedSession.Session;
    }

    public async Task<Bookmark?> GetByUserAndHaikuAsync(Guid userId, Guid haikuId) =>
        await _session.Query<Bookmark>()
            .FirstOrDefaultAsync(b => b.User.Id == userId && b.Poem.Id == haikuId);

    public async Task SaveAsync(Bookmark bookmark)
    {
        await _session.SaveOrUpdateAsync(bookmark);
        await _session.FlushAsync();
    }

    public async Task DeleteAsync(Bookmark bookmark)
    {
        await _session.DeleteAsync(bookmark);
        await _session.FlushAsync();
    }
}
