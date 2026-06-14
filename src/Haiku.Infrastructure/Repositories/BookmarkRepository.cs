using Haiku.Domain.Entities;
using Haiku.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Haiku.Infrastructure.Repositories;

/// <summary>
/// Persistence store for <see cref="Bookmark"/> entities using EF Core.
/// </summary>
public class BookmarkRepository : IBookmarkRepository
{
    private readonly HaikuDbContext _db;

    /// <summary>
    /// Initializes a new instance of the <see cref="BookmarkRepository"/> class.
    /// </summary>
    /// <param name="db">The database context.</param>
    public BookmarkRepository(HaikuDbContext db)
    {
        _db = db;
    }

    /// <inheritdoc/>
    public async Task<Bookmark?> GetByUserAndPoemAsync(Guid userId, Guid poemId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _db.Bookmarks.FirstOrDefaultAsync(b => b.UserId == userId && b.PoemId == poemId, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task SaveAsync(Bookmark bookmark, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var entry = _db.Entry(bookmark);
        // If the entity is detached it is new and must be added to the change tracker.
        // If already tracked (e.g. retrieved earlier in the same scope), SaveChanges
        // picks up modifications automatically.
        if (entry.State == EntityState.Detached)
        {
            _db.Bookmarks.Add(bookmark);
        }
        await _db.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(Bookmark bookmark, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _db.Bookmarks.Remove(bookmark);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
