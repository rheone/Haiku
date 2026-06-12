using Haiku.Domain.Entities;
using Haiku.Domain.Interfaces;
using NHibernate;
using NHibernate.Linq;

namespace Haiku.Infrastructure.Repositories;

public class TagRepository : ITagRepository
{
    private readonly ISession _session;

    public TagRepository(ScopedSession scopedSession)
    {
        _session = scopedSession.Session;
    }

    public async Task<Tag?> GetByNameAsync(string name) =>
        await _session.Query<Tag>().FirstOrDefaultAsync(t => t.Name == name);

    public async Task<Tag> GetOrCreateAsync(string name)
    {
        var tag = await GetByNameAsync(name);
        if (tag != null) return tag;

        tag = new Tag { Name = name };
        await _session.SaveAsync(tag);
        await _session.FlushAsync();
        return tag;
    }
}
