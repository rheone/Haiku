using Microsoft.EntityFrameworkCore;

namespace Haiku.Modules.Poems.Infrastructure;

/// <summary>
/// Persistence store for <see cref="Poem"/> entities using EF Core.
/// </summary>
public class PoemRepository : IPoemRepository
{
    private readonly HaikuDbContext _db;

    /// <summary>
    /// Initializes a new instance of the <see cref="PoemRepository"/> class.
    /// </summary>
    /// <param name="db">The database context.</param>
    public PoemRepository(HaikuDbContext db)
    {
        _db = db;
    }

    /// <inheritdoc/>
    public async Task<Poem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _db.Poems.FindAsync(new object[] { id }, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task SaveAsync(Poem poem, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var entry = _db.Entry(poem);
        // If the entity is detached it is new and must be added to the change tracker.
        // If already tracked, SaveChanges persists modifications automatically.
        if (entry.State == EntityState.Detached)
        {
            _db.Poems.Add(poem);
        }
        await _db.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(Poem poem, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _db.Poems.Remove(poem);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
