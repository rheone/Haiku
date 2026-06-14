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

    /// <summary>
    /// Retrieves a vote cast by a specific user on a specific poem.
    /// </summary>
    /// <param name="userId">The unique identifier of the user who voted.</param>
    /// <param name="poemId">The unique identifier of the poem that was voted on.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>The vote if found; otherwise <c>null</c>.</returns>
    public async Task<Vote?> GetByUserAndPoemAsync(Guid userId, Guid poemId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _db.Votes.FirstOrDefaultAsync(v => v.UserId == userId && v.PoemId == poemId, cancellationToken);
    }

    /// <summary>
    /// Persists a new vote or saves changes to an existing tracked vote.
    /// </summary>
    /// <param name="vote">The vote entity to save.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>A task representing the asynchronous save operation.</returns>
    public async Task SaveAsync(Vote vote, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var entry = _db.Entry(vote);
        if (entry.State == EntityState.Detached)
        {
            _db.Votes.Add(vote);
        }
        await _db.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Deletes a vote entity from the database.
    /// </summary>
    /// <param name="vote">The vote entity to remove.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>A task representing the asynchronous delete operation.</returns>
    public async Task DeleteAsync(Vote vote, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _db.Votes.Remove(vote);
        await _db.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Calculates the net score (upvotes minus downvotes) for a poem.
    /// </summary>
    /// <param name="poemId">The unique identifier of the poem.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
    /// <returns>The net score as an integer.</returns>
    public async Task<int> GetNetScoreAsync(Guid poemId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _db.Votes.Where(v => v.PoemId == poemId).SumAsync(v => (int)v.Value, cancellationToken);
    }
}
