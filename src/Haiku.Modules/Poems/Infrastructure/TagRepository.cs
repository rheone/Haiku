using Microsoft.EntityFrameworkCore;

namespace Haiku.Modules.Poems.Infrastructure;

/// <summary>
/// Persistence store for <see cref="Tag"/> entities using EF Core.
/// </summary>
public class TagRepository : ITagRepository
{
    private readonly HaikuDbContext _db;

    /// <summary>
    /// Initializes a new instance of the <see cref="TagRepository"/> class.
    /// </summary>
    /// <param name="db">The database context.</param>
    public TagRepository(HaikuDbContext db)
    {
        _db = db;
    }

    /// <inheritdoc/>
    public async Task<Tag?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _db.Tags.FirstOrDefaultAsync(t => t.Name == name, cancellationToken);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// <para>Concurrent requests for the same new tag name are handled gracefully:
    /// a unique index violation on save triggers a re-query that returns the row
    /// inserted by the winning request. No explicit locking is required.</para>
    /// </remarks>
    public async Task<Tag> GetOrCreateAsync(string name, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var tag = await GetByNameAsync(name, cancellationToken);
        if (tag != null)
        {
            return tag;
        }

        tag = new Tag { Name = name };
        _db.Tags.Add(tag);

        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        // Another request inserted the same tag between our check and save.
        // The unique index violation is expected; return the row that won the race.
        catch (DbUpdateException) when (_db.Tags.Any(t => t.Name == name))
        {
            return (await _db.Tags.FirstAsync(t => t.Name == name, cancellationToken))!;
        }

        return tag;
    }
}
