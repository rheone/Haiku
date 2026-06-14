using Haiku.Domain.Entities;
using Haiku.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Haiku.Infrastructure.Repositories;

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

    /// <summary>
    /// Retrieves a love (heart) record for a specific user on a specific poem.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="poemId">The unique identifier of the poem.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>The love record if found; otherwise <c>null</c>.</returns>
    public async Task<Love?> GetByUserAndPoemAsync(Guid userId, Guid poemId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _db.Loves.FirstOrDefaultAsync(l => l.UserId == userId && l.PoemId == poemId, cancellationToken);
    }

    /// <summary>
    /// Persists a new love record or saves changes to an existing tracked record.
    /// </summary>
    /// <param name="love">The love entity to save.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>A task representing the asynchronous save operation.</returns>
    public async Task SaveAsync(Love love, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var entry = _db.Entry(love);
        if (entry.State == EntityState.Detached)
        {
            _db.Loves.Add(love);
        }
        await _db.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Deletes a love record from the database.
    /// </summary>
    /// <param name="love">The love entity to remove.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>A task representing the asynchronous delete operation.</returns>
    public async Task DeleteAsync(Love love, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _db.Loves.Remove(love);
        await _db.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Counts the total number of loves (hearts) received by a poem.
    /// </summary>
    /// <param name="poemId">The unique identifier of the poem.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>The total love count.</returns>
    public async Task<int> GetLoveCountAsync(Guid poemId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _db.Loves.CountAsync(l => l.PoemId == poemId, cancellationToken);
    }
}
