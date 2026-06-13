using Haiku.Domain.Entities;
using Haiku.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Haiku.Infrastructure.Repositories;

/// <summary>
/// Persistence store for <see cref="Poem"/> (haiku) entities using EF Core.
/// </summary>
public class HaikuRepository : IHaikuRepository
{
    private readonly HaikuDbContext _db;

    /// <summary>
    /// Initializes a new instance of the <see cref="HaikuRepository"/> class.
    /// </summary>
    /// <param name="db">The database context.</param>
    public HaikuRepository(HaikuDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Retrieves a poem by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the poem.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>The poem if found; otherwise <c>null</c>.</returns>
    public async Task<Poem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _db.Poems.FindAsync(new object[] { id }, cancellationToken);
    }

    /// <summary>
    /// Persists a new poem or saves changes to an existing tracked poem.
    /// </summary>
    /// <param name="poem">The poem entity to save.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>A task representing the asynchronous save operation.</returns>
    public async Task SaveAsync(Poem poem, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var entry = _db.Entry(poem);
        if (entry.State == EntityState.Detached)
        {
            _db.Poems.Add(poem);
        }
        await _db.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Deletes a poem entity from the database.
    /// </summary>
    /// <param name="poem">The poem entity to remove.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>A task representing the asynchronous delete operation.</returns>
    public async Task DeleteAsync(Poem poem, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _db.Poems.Remove(poem);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
