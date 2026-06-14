using Haiku.Domain.Entities;
using Haiku.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Haiku.Infrastructure.Repositories;

/// <summary>
/// Persistence store for <see cref="Vote"/> entities using EF Core.
/// </summary>
public class VoteRepository : IVoteRepository
{
    private readonly HaikuDbContext _db;

    /// <summary>
    /// Initializes a new instance of the <see cref="VoteRepository"/> class.
    /// </summary>
    /// <param name="db">The database context.</param>
    public VoteRepository(HaikuDbContext db)
    {
        _db = db;
    }

    /// <inheritdoc/>
    public async Task<Vote?> GetByUserAndPoemAsync(Guid userId, Guid poemId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _db.Votes.FirstOrDefaultAsync(v => v.UserId == userId && v.PoemId == poemId, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task SaveAsync(Vote vote, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var entry = _db.Entry(vote);
        // If the entity is detached it is new and must be added to the change tracker.
        // If already tracked, SaveChanges persists modifications automatically.
        if (entry.State == EntityState.Detached)
        {
            _db.Votes.Add(vote);
        }
        await _db.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(Vote vote, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _db.Votes.Remove(vote);
        await _db.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<int> GetNetScoreAsync(Guid poemId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _db.Votes.Where(v => v.PoemId == poemId).SumAsync(v => (int)v.Value, cancellationToken);
    }
}
