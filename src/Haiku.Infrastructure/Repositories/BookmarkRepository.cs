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

    /// <summary>
    /// Retrieves a bookmark record for a specific user on a specific poem.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="poemId">The unique identifier of the poem.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>The bookmark if found; otherwise <c>null</c>.</returns>
    public async Task<Bookmark?> GetByUserAndPoemAsync(Guid userId, Guid poemId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _db.Bookmarks.FirstOrDefaultAsync(b => b.UserId == userId && b.PoemId == poemId, cancellationToken);
    }

    /// <summary>
    /// Persists a new bookmark or saves changes to an existing tracked bookmark.
    /// </summary>
    /// <param name="bookmark">The bookmark entity to save.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>A task representing the asynchronous save operation.</returns>
    public async Task SaveAsync(Bookmark bookmark, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var entry = _db.Entry(bookmark);
        if (entry.State == EntityState.Detached)
        {
            _db.Bookmarks.Add(bookmark);
        }
        await _db.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Deletes a bookmark record from the database.
    /// </summary>
    /// <param name="bookmark">The bookmark entity to remove.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>A task representing the asynchronous delete operation.</returns>
    public async Task DeleteAsync(Bookmark bookmark, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _db.Bookmarks.Remove(bookmark);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
