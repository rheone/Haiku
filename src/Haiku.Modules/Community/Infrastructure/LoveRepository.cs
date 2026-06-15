using Microsoft.EntityFrameworkCore;

namespace Haiku.Modules.Community.Infrastructure;

/// <summary>
/// Persistence store for <see cref="Love"/> entities using EF Core.
/// </summary>
public class LoveRepository : ILoveRepository
{
    private readonly HaikuDbContext _db;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoveRepository"/> class.
    /// </summary>
    /// <param name="db">The database context.</param>
    public LoveRepository(HaikuDbContext db)
    {
        _db = db;
    }

    /// <inheritdoc/>
    public async Task<Love?> GetByUserAndPoemAsync(Guid userId, Guid poemId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _db.Loves.FirstOrDefaultAsync(l => l.UserId == userId && l.PoemId == poemId, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task SaveAsync(Love love, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var entry = _db.Entry(love);
        // If the entity is detached it is new and must be added to the change tracker.
        // If already tracked, SaveChanges persists modifications automatically.
        if (entry.State == EntityState.Detached)
        {
            _db.Loves.Add(love);
        }
        await _db.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(Love love, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _db.Loves.Remove(love);
        await _db.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<int> GetLoveCountAsync(Guid poemId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _db.Loves.CountAsync(l => l.PoemId == poemId, cancellationToken);
    }
}
